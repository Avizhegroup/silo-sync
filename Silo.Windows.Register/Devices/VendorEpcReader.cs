using Silo.Windows.Register.Config;

namespace Silo.Windows.Register.Devices;

/// <summary>
/// DEVICE INTEGRATION POINT: wrapper for vendor drivers (6061 RfidApiLib, Tiny UhfReader_API_HID, T64 VHLib).
/// The vendor DLLs are not part of the repository; implement Start/Stop here and raise EpcRead with the raw EPC.
/// </summary>
public sealed class VendorEpcReader(DeskReaderType type, string port, int power) : IEpcReader
{
    public string Name => $"{type} ({port}, power {power})";
    public bool IsRunning { get; private set; }
    public event EventHandler<string>? EpcRead;

    public void Start() => throw new NotSupportedException(
        $"The {type} vendor driver is not installed. Add the vendor DLL and implement VendorEpcReader, or choose the Serial or Simulated reader.");

    public void Stop() => IsRunning = false;

    public void Dispose() => Stop();

    internal void Publish(string epc) => EpcRead?.Invoke(this, epc);
}
