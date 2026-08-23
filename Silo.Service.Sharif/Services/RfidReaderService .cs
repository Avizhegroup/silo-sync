
using Silo.Service.Sharif.Dtos;

namespace Silo.Service.Sharif;

public class RfidReaderService : IDisposable
{
    private readonly ILogger<RfidReaderService> _logger;

    public RfidReaderService(ILogger<RfidReaderService> logger)
    {
        _logger = logger;
        _uhf = UHFAPI.getInstance();
    }

    private readonly UHFAPI _uhf;
    private readonly object _sync = new object();
    private bool disposed;
    private ConnectionTypeEnum _connectionType = ConnectionTypeEnum.None;
    private CancellationTokenSource _inventoryCts;
    private bool isInventoryRunning;

    #region Connection Methods
    public void Connect(string ip, uint port)
    {
        EnsureNotDisposed();

        if (_connectionType != ConnectionTypeEnum.None)
        {
            _logger.LogInformation("Already connected.");
        }

        if (!_uhf.TcpConnect(ip, port))
        {
            _logger.LogWarning($"TCP Connection failed to {ip}:{port}");
        }

        _connectionType = ConnectionTypeEnum.Tcp;
    }

    public void ConnectUsb()
    {
        EnsureNotDisposed();

        if (_connectionType != ConnectionTypeEnum.None)
        {
            _logger.LogInformation("Already connected.");
        }

        if (!_uhf.OpenUsb())
        {
            _logger.LogWarning("USB Connection failed.");
        }

        _connectionType = ConnectionTypeEnum.Usb;
    }

    public void Disconnect()
    {
        StopInventory();

        if (_connectionType == ConnectionTypeEnum.None) return;
        switch (_connectionType)
        {
            case ConnectionTypeEnum.Tcp: _uhf.TcpDisconnect(); break;
            case ConnectionTypeEnum.Serial: _uhf.Close(); break;
            case ConnectionTypeEnum.Usb: _uhf.CloseUsb(); break;
        }
        _connectionType = ConnectionTypeEnum.None;

    }
    #endregion

    #region Power Management
    public void SetPower(byte save, byte power)
    {
        EnsureConnected();
        if (power < 1 || power > 30) throw new ArgumentOutOfRangeException(nameof(power), "Power 1-30 dBm");

        if (!_uhf.SetPower(save, power))
        {
            _logger.LogWarning("SetPower failed. Save: {Save}, Power: {Power}", save, power);
        }

    }

    public byte GetPower()
    {
        EnsureConnected();

        byte power = 0;

        if (!_uhf.GetPower(ref power))
        {
            _logger.LogWarning("GetPower failed while reading reader power.");
        }

        return power;

    }
    #endregion

    #region Continuous Inventory (The Core Logic)
    public void StartInventory()
    {
        EnsureConnected();

        _uhf.StartInventory();

        isInventoryRunning = true;
    }

    public UHFTAGInfo? ReadTag()
    {
        var res = _uhf.uhfGetReceived();

        return res;
    }

    public void StopInventory()
    {
        if (!isInventoryRunning) return;

        _inventoryCts?.Cancel();

        _uhf.StopInventory();

        isInventoryRunning = false;
    }
    #endregion

    #region Helpers & Cleanup
    public void Dispose()
    {
        if (disposed)
        {
            return;
        }

        Disconnect();

        _inventoryCts?.Dispose();

        disposed = true;

        GC.SuppressFinalize(this);
    }

    private void EnsureConnected()
    {
        EnsureNotDisposed();
        if (_connectionType == ConnectionTypeEnum.None)
        {
            _logger.LogWarning("Reader not connected.");
        }
    }

    private void EnsureNotDisposed()
    {
        if (disposed)
        {
            _logger.LogWarning("An operation was attempted on a disposed {ServiceName}.", nameof(RfidReaderService));
        }
    }
    #endregion
}
