using System.Runtime.InteropServices;

namespace Silo.Windows.Register.Devices.Vendor;

/// <summary>
/// "T64" USB reader (native VHLib.dll, x64). Mirrors the legacy ConnectToT64UHFReader: connect over USB,
/// read version / parameters (3 attempts each), write the basic parameters (power, EPC C1G2), then loop
/// EPC1G2_ReadLabelID. The ignore list is applied by ReaderManager.
/// </summary>
internal sealed class T64Driver(int power) : PollingVendorDriver
{
    private const int Ok = 0;
    private const int Attempts = 3;
    private const int Rs485Address = 0;
    private const int PollIntervalMilliseconds = 10;

    private int _scanner = -1;
    private readonly byte[] _mask = new byte[512];
    private readonly byte[] _ids = new byte[7680];

    protected override string DriverName => "T64";

    protected override void Open()
    {
        var scanner = -1;
        Check(VhNative.VH_ConnectScannerUsb(ref scanner), "connect over USB");
        _scanner = scanner;

        try
        {
            int hardware = 0, software = 0;
            Retry("read version", () => VhNative.VH_GetVersion(scanner, ref hardware, ref software));

            var basic = new VhNative.ReaderBasicParam();
            Retry("read basic parameters", () => VhNative.ReadBasicParam(scanner, ref basic));

            byte frequency = 0;
            Retry("read frequency range", () => VhNative.GetFrequencyRange(scanner, ref frequency));

            var auto = new VhNative.ReaderAutoParam();
            Retry("read auto parameters", () => VhNative.ReadAutoParam(scanner, ref auto));

            var readerId = new byte[33];
            Retry("read reader id", () => VhNative.GetReaderID(scanner, readerId));

            var sim = new VhNative.SimParam();
            Retry("read simulation parameters", () => VhNative.ReadSimParam(scanner, ref sim));

            basic.Min_Frequence = 1;
            basic.Max_Frequence = 11;
            basic.Power = (byte)Math.Clamp(power, 0, 63);
            basic.TagType = 0x04; // EPC C1G2
            Retry("write basic parameters", () => VhNative.WriteBasicParam(scanner, ref basic));
        }
        catch
        {
            VhNative.VH_DisconnectScannerUsb(scanner);
            _scanner = -1;
            throw;
        }
    }

    protected override void PollOnce(Action<string> onEpc)
    {
        var counter = 0;
        var result = VhNative.EPC1G2_ReadLabelID(_scanner, 1, 0, 0, _mask, _ids, ref counter, Rs485Address);
        if (result == Ok)
        {
            // _ids[0] holds the EPC length in 16-bit words; the EPC bytes follow (first tag only, as in the legacy app).
            var words = _ids[0];
            if (words > 0)
            {
                onEpc(ToHex(_ids.AsSpan(1, words * 2)));
            }
        }

        Thread.Sleep(PollIntervalMilliseconds);
    }

    protected override void Close()
    {
        var scanner = Interlocked.Exchange(ref _scanner, -1);
        if (scanner != -1)
        {
            VhNative.VH_DisconnectScannerUsb(scanner);
        }
    }

    private static void Retry(string step, Func<ushort> call)
    {
        ushort result = 0;
        for (var attempt = 0; attempt < Attempts; attempt++)
        {
            result = call();
            if (result == Ok)
            {
                return;
            }
        }

        Check(result, step);
    }

    private static void Check(ushort result, string step)
    {
        if (result != Ok)
        {
            throw new InvalidOperationException($"T64 reader failed to {step} (code 0x{result:X2}).");
        }
    }

    private static class VhNative
    {
        private const string Library = "VHLib.dll";

        [DllImport(Library, CharSet = CharSet.Ansi)]
        public static extern ushort VH_ConnectScannerUsb(ref int scanner);

        [DllImport(Library, CharSet = CharSet.Ansi)]
        public static extern ushort VH_DisconnectScannerUsb(int scanner);

        [DllImport(Library, CharSet = CharSet.Ansi)]
        public static extern ushort VH_GetVersion(int scanner, ref int hardware, ref int software);

        [DllImport(Library, CharSet = CharSet.Ansi)]
        public static extern ushort ReadBasicParam(int scanner, ref ReaderBasicParam parameter);

        [DllImport(Library, CharSet = CharSet.Ansi)]
        public static extern ushort WriteBasicParam(int scanner, ref ReaderBasicParam parameter);

        [DllImport(Library, CharSet = CharSet.Ansi)]
        public static extern ushort ReadAutoParam(int scanner, ref ReaderAutoParam parameter);

        [DllImport(Library, CharSet = CharSet.Ansi)]
        public static extern ushort ReadSimParam(int scanner, ref SimParam parameter);

        [DllImport(Library, CharSet = CharSet.Ansi)]
        public static extern ushort GetFrequencyRange(int scanner, ref byte frequency);

        [DllImport(Library, CharSet = CharSet.Ansi)]
        public static extern ushort GetReaderID(int scanner, byte[] readerId);

        [DllImport(Library, CharSet = CharSet.Ansi)]
        public static extern ushort EPC1G2_ReadLabelID(int scanner, int memory, int pointer, int length, byte[] mask, byte[] idBuffer, ref int counter, int address);

        // Field layouts are fixed by the vendor DLL - do not reorder or resize.
        [StructLayout(LayoutKind.Sequential)]
        public struct ReaderBasicParam
        {
            public byte BaudRate;
            public byte Power;
            public byte Min_Frequence;
            public byte Max_Frequence;
            public byte Reserve5;
            public byte WorkMode;
            public byte ReaderAddress;
            public byte NumofCard;
            public byte TagType;
            public byte ReadDuration;
            public byte ReadTimes;
            public byte EnableBuzzer;
            public byte IP1, IP2, IP3, IP4;
            public byte Port1, Port2;
            public byte Mask1, Mask2, Mask3, Mask4;
            public byte Gateway1, Gateway2, Gateway3, Gateway4;
            public byte MAC1, MAC2, MAC3, MAC4, MAC5, MAC6;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct ReaderAutoParam
        {
            public byte AutoMode;
            public byte TimeH;
            public byte TimeL;
            public byte Interval;
            public byte NumH;
            public byte NumL;
            public byte OutputManner;
            public byte OutInterface;
            public byte WiegandWidth;
            public byte WiegandInterval;
            public byte ID_Start;
            public byte IDPosition;
            public byte Report_Interval;
            public byte Report_Condition;
            public byte Report_Output;
            public byte Antenna;
            public byte TriggerMode;
            public byte HostIP1, HostIP2, HostIP3, HostIP4;
            public byte Port1, Port2;
            public byte Reserve24;
            public byte ArgentinaSim;
            public byte CardTime1;
            public byte CardTime2;
            public byte ArgentinaMode;
            public byte AnSleepTime;
            public byte Alarm;
            public byte Reserve31;
            public byte EnableRelay;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct SimParam
        {
            public byte DataFormat;
            public byte IsPerfix;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
            public byte[] PerfixCode;
            public byte IsEnter;
            public byte StartAddress;
            public byte DataLen;
            public byte OutputInterval;
        }
    }
}
