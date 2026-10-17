using Silo.Windows.Register.Config;

namespace Silo.Windows.Register.Devices;

public enum ReadMode
{
    Single = 1,
    Continuous = 2
}

/// <summary>Owns the active reader; normalizes EPCs (24 hex), applies the ignore list and de-duplicates.</summary>
public sealed class ReaderManager(RegisterSettings settings) : IDisposable
{
    private readonly HashSet<string> _seen = new(StringComparer.OrdinalIgnoreCase);
    private readonly object _gate = new();
    private IEpcReader? _reader;
    private ReadMode _mode;

    public event EventHandler<string>? EpcRead;
    public event EventHandler<string>? StatusChanged;

    public bool IsRunning => _reader?.IsRunning == true;

    public void Start(ReadMode mode)
    {
        Stop();
        _mode = mode;
        var reader = Create();
        reader.EpcRead += OnRawEpc;
        try
        {
            reader.Start();
        }
        catch
        {
            reader.EpcRead -= OnRawEpc;
            reader.Dispose();
            throw;
        }
        _reader = reader;
        StatusChanged?.Invoke(this, $"Reader started: {reader.Name} ({mode})");
    }

    public void Stop()
    {
        var reader = Interlocked.Exchange(ref _reader, null);
        if (reader is null)
        {
            return;
        }

        reader.EpcRead -= OnRawEpc;
        reader.Dispose();
        lock (_gate)
        {
            _seen.Clear();
        }
        StatusChanged?.Invoke(this, "Reader stopped");
    }

    public void ClearSeen()
    {
        lock (_gate)
        {
            _seen.Clear();
        }
    }

    private IEpcReader Create() => settings.DeskReaderType switch
    {
        DeskReaderType.Serial => new SerialEpcReader(settings.DeskUHFReaderPort, settings.BaudRate),
        DeskReaderType.Simulated => new SimulatedEpcReader(),
        _ => new VendorEpcReader(settings.DeskReaderType, settings.DeskUHFReaderPort, settings.ReaderPower)
    };

    private void OnRawEpc(object? sender, string raw)
    {
        var epc = Normalize(raw);
        if (epc is null)
        {
            return;
        }

        if (settings.DeskReaderType == DeskReaderType.T64 && settings.GetIgnoredEpcs().Contains(epc))
        {
            return;
        }

        lock (_gate)
        {
            if (!_seen.Add(epc))
            {
                return;
            }
        }

        EpcRead?.Invoke(this, epc);
        if (_mode == ReadMode.Single)
        {
            Task.Run(Stop);
        }
    }

    private string? Normalize(string raw)
    {
        var value = raw.Trim();
        if (value.Length > 24)
        {
            value = settings.DeskReaderType == DeskReaderType.Tiny ? value[^24..] : value[..24];
        }

        return value.Length == 24 && value.All(Uri.IsHexDigit) ? value.ToUpperInvariant() : null;
    }

    public void Dispose() => Stop();
}
