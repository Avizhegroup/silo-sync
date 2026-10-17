using RfidApiLib;

namespace Silo.Windows.Register.Devices.Vendor;

/// <summary>
/// "6061" desk reader (managed vendor library RfidApiLib.dll, serial protocol).
/// Mirrors the legacy ConnectToDeskUHFReader: OpenCommPort, SetRf(power, 2), poll EpcMultiTagIdentify
/// (12-byte EPCs, 24 hex characters).
/// </summary>
internal sealed class Rfid6061Driver(string port, int power) : PollingVendorDriver
{
    private const byte FrequencyMode = 2;
    private const int PollIntervalMilliseconds = 200;
    private const int MaximumTags = 100;
    private const int EpcBytes = 12;

    private RfidApi? _api;

    protected override string DriverName => "6061";

    protected override void Open()
    {
        var api = new RfidApi();
        var status = api.OpenCommPort(port);
        if (status != 0)
        {
            throw new InvalidOperationException($"Cannot open reader on {port} (status {status}).");
        }

        byte major = 0;
        byte minor = 0;
        api.GetFirmwareVersion(ref major, ref minor);
        api.ClearReceiveBuf();
        api.SetRf((byte)Math.Clamp(power, 0, 255), FrequencyMode);
        api.ClearIdBuf();
        _api = api;
    }

    protected override void PollOnce(Action<string> onEpc)
    {
        var api = _api ?? throw new InvalidOperationException("Reader is not open.");
        var buffer = new byte[MaximumTags, EpcBytes];
        byte count = 0;
        byte flag = 0;
        api.EpcMultiTagIdentify(ref buffer, ref count, ref flag);

        for (var tag = 0; tag < count && tag < MaximumTags; tag++)
        {
            var epc = new byte[EpcBytes];
            for (var index = 0; index < EpcBytes; index++)
            {
                epc[index] = buffer[tag, index];
            }
            onEpc(ToHex(epc));
        }

        api.ClearReceiveBuf();
        api.ClearIdBuf();
        Thread.Sleep(PollIntervalMilliseconds);
    }

    protected override void Close()
    {
        var api = Interlocked.Exchange(ref _api, null);
        if (api is null)
        {
            return;
        }

        api.CloseCommPort();
        api.ClearIdBuf();
    }
}
