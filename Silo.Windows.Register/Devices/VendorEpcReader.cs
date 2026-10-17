using Silo.Windows.Register.Config;
using Silo.Windows.Register.Devices.Vendor;

namespace Silo.Windows.Register.Devices;

/// <summary>Optional interface for readers that can report asynchronous device failures.</summary>
public interface IReaderFaultSource
{
    event EventHandler<string>? Faulted;
}

/// <summary>
/// Desk readers driven by the vendor libraries: 6061 (RfidApiLib), Tiny (UhfReader_API_HID) and T64 (VHLib).
/// Raw EPC strings are published as read; <see cref="ReaderManager"/> normalizes (24 hex), de-duplicates and
/// applies the T64 ignore list.
/// </summary>
public sealed class VendorEpcReader(DeskReaderType type, string port, int power) : IEpcReader, IReaderFaultSource
{
    private PollingVendorDriver? _driver;

    public string Name => $"{type} ({(type == DeskReaderType.Model6061 ? port : "USB")}, power {power})";
    public bool IsRunning { get; private set; }
    public event EventHandler<string>? EpcRead;
    public event EventHandler<string>? Faulted;

    public void Start()
    {
        if (IsRunning)
        {
            return;
        }

        PollingVendorDriver driver = type switch
        {
            DeskReaderType.Model6061 => new Rfid6061Driver(port, power),
            DeskReaderType.Tiny => new TinyHidDriver(),
            DeskReaderType.T64 => new T64Driver(power),
            _ => throw new NotSupportedException($"'{type}' is not a vendor reader type.")
        };

        driver.Faulted += OnDriverFaulted;
        try
        {
            driver.Start(raw => EpcRead?.Invoke(this, raw));
        }
        catch (Exception exception) when (exception is DllNotFoundException or BadImageFormatException or EntryPointNotFoundException)
        {
            driver.Faulted -= OnDriverFaulted;
            driver.Dispose();
            throw new InvalidOperationException(
                $"The {type} vendor library could not be loaded ({exception.Message}). Check that the vendor DLLs are next to the application and that it runs as x64.",
                exception);
        }
        catch
        {
            driver.Faulted -= OnDriverFaulted;
            driver.Dispose();
            throw;
        }

        _driver = driver;
        IsRunning = true;
    }

    public void Stop()
    {
        var driver = Interlocked.Exchange(ref _driver, null);
        IsRunning = false;
        if (driver is null)
        {
            return;
        }

        driver.Faulted -= OnDriverFaulted;
        driver.Dispose();
    }

    public void Dispose() => Stop();

    private void OnDriverFaulted(object? sender, string message) => Faulted?.Invoke(this, message);
}
