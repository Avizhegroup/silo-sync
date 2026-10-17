using Silo.Windows.Register.Config;

namespace Silo.Windows.Register.Devices;

/// <summary>Collects keyboard-wedge scanner keystrokes into a barcode and applies the splitter rule.</summary>
public sealed class BarcodeWedgeBuffer : IDisposable
{
    private readonly RegisterSettings _settings;
    private readonly System.Windows.Forms.Timer _idle = new();
    private readonly System.Text.StringBuilder _buffer = new();

    public BarcodeWedgeBuffer(RegisterSettings settings)
    {
        _settings = settings;
        _idle.Interval = Math.Max(20, settings.BarcodeIdleMilliseconds);
        _idle.Tick += (_, _) => Flush();
    }

    public event EventHandler<string>? BarcodeRead;

    public void Append(char key)
    {
        if (key is '\r' or '\n')
        {
            Flush();
            return;
        }

        if (char.IsControl(key))
        {
            return;
        }

        _buffer.Append(key);
        _idle.Stop();
        _idle.Start();
        if (_settings.SerialLength > 0 && _buffer.Length == _settings.SerialLength)
        {
            Flush();
        }
    }

    public string Split(string barcode)
    {
        if (string.IsNullOrEmpty(_settings.BarcodeSpliter))
        {
            return barcode;
        }

        var parts = barcode.Split(_settings.BarcodeSpliter[0]);
        return parts.Length switch
        {
            3 => parts[2],
            2 => parts[1],
            _ => barcode
        };
    }

    private void Flush()
    {
        _idle.Stop();
        if (_buffer.Length == 0)
        {
            return;
        }

        var value = _buffer.ToString();
        _buffer.Clear();
        BarcodeRead?.Invoke(this, Split(value));
    }

    public void Dispose() => _idle.Dispose();
}
