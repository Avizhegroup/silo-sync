using System.Runtime.InteropServices;
using System.Text;

namespace Silo.Windows.Register.Devices.Vendor;

/// <summary>
/// "Tiny" USB-HID reader (native UhfReader_API_HID.dll, x64). Mirrors the legacy ConnectToTinyUHFReader:
/// first HID found, 57600 baud with CRC (flag 0x05), inventory with anti-collision 0x01 and Q = 3,
/// then ReadInventory in a loop.
/// </summary>
internal sealed class TinyHidDriver : PollingVendorDriver
{
    private const string Library = "UhfReader_API_HID.dll";
    private const byte FlagCrc = 0x05;      // 57600 baud (0x04) | CRC (0x01)
    private const byte AntiCollision = 0x01;
    private const byte InitialQ = 0x03;
    private const int ResultBufferSize = 255;

    private IntPtr _handle;
    private readonly byte[] _length = new byte[1];
    private readonly byte[] _uii = new byte[ResultBufferSize];

    protected override string DriverName => "Tiny";

    protected override void Open()
    {
        var serials = new StringBuilder(1024);
        var count = UhfSearchHids(serials);
        if (count <= 0 || serials.Length == 0)
        {
            throw new InvalidOperationException("No Tiny USB reader found.");
        }

        var device = serials.ToString().Split('|', StringSplitOptions.RemoveEmptyEntries)[0];
        if (!UhfReaderConnect(ref _handle, device, FlagCrc))
        {
            throw new InvalidOperationException($"Cannot connect to the Tiny reader '{device}'.");
        }

        if (!UhfStartInventory(_handle, AntiCollision, InitialQ, FlagCrc))
        {
            UhfReaderDisconnect(ref _handle, FlagCrc);
            _handle = IntPtr.Zero;
            throw new InvalidOperationException("The Tiny reader refused to start inventory.");
        }
    }

    protected override void PollOnce(Action<string> onEpc)
    {
        if (UhfReadInventory(_handle, _length, _uii))
        {
            var length = Math.Min((int)_length[0], ResultBufferSize);
            if (length > 0)
            {
                onEpc(ToHex(_uii.AsSpan(0, length)));
            }
        }
        else
        {
            Thread.Sleep(5);
        }
    }

    protected override void Close()
    {
        if (_handle == IntPtr.Zero)
        {
            return;
        }

        UhfStopOperation(_handle, FlagCrc);
        Thread.Sleep(300);
        UhfReaderDisconnect(ref _handle, FlagCrc);
        _handle = IntPtr.Zero;
    }

    [DllImport(Library, CallingConvention = CallingConvention.Winapi, CharSet = CharSet.Ansi)]
    private static extern int UhfSearchHids(StringBuilder serials);

    [DllImport(Library, CallingConvention = CallingConvention.Winapi, CharSet = CharSet.Ansi)]
    private static extern bool UhfReaderConnect(ref IntPtr hCom, string port, byte flagCrc);

    [DllImport(Library, CallingConvention = CallingConvention.Winapi)]
    private static extern bool UhfReaderDisconnect(ref IntPtr hCom, byte flagCrc);

    [DllImport(Library, CallingConvention = CallingConvention.Winapi)]
    private static extern bool UhfStartInventory(IntPtr hCom, byte flagAnti, byte initQ, byte flagCrc);

    [DllImport(Library, CallingConvention = CallingConvention.Winapi)]
    private static extern bool UhfReadInventory(IntPtr hCom, byte[] length, byte[] uii);

    [DllImport(Library, CallingConvention = CallingConvention.Winapi)]
    private static extern bool UhfStopOperation(IntPtr hCom, byte flagCrc);
}
