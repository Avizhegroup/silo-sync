namespace Silo.Windows.Register.Devices.Vendor;

/// <summary>
/// Base class for the vendor desk-reader drivers: opens the device, then polls it on a dedicated
/// background thread and reports every raw EPC string (hex) to the caller.
/// </summary>
internal abstract class PollingVendorDriver : IDisposable
{
    private Thread? _thread;
    private volatile bool _running;
    private bool _opened;
    private string? _lastFault;

    public event EventHandler<string>? Faulted;

    protected abstract string DriverName { get; }

    /// <summary>Opens the connection. Must throw when the device cannot be opened.</summary>
    protected abstract void Open();

    /// <summary>Reads once from the device and calls <paramref name="onEpc"/> for each tag found.</summary>
    protected abstract void PollOnce(Action<string> onEpc);

    /// <summary>Closes the connection. Must not throw for a device that is already closed.</summary>
    protected abstract void Close();

    public void Start(Action<string> onEpc)
    {
        Open();
        _opened = true;
        _running = true;
        _thread = new Thread(() => Loop(onEpc)) { IsBackground = true, Name = $"{DriverName} reader" };
        _thread.Start();
    }

    private void Loop(Action<string> onEpc)
    {
        while (_running)
        {
            try
            {
                PollOnce(onEpc);
            }
            catch (Exception exception)
            {
                // Report each distinct failure once, then keep polling so a transient USB glitch recovers.
                if (!string.Equals(_lastFault, exception.Message, StringComparison.Ordinal))
                {
                    _lastFault = exception.Message;
                    Faulted?.Invoke(this, $"{DriverName}: {exception.Message}");
                }
                Thread.Sleep(500);
            }
        }
    }

    public void Dispose()
    {
        _running = false;
        // Wait for the poll loop first so no driver call runs after Close().
        _thread?.Join(3000);
        _thread = null;
        if (_opened)
        {
            _opened = false;
            try
            {
                Close();
            }
            catch (Exception exception)
            {
                Faulted?.Invoke(this, $"{DriverName}: close failed - {exception.Message}");
            }
        }
    }

    protected static string ToHex(ReadOnlySpan<byte> bytes) => Convert.ToHexString(bytes);
}
