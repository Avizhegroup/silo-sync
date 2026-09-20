using Silo.Service.Sharif.Dtos;
using Silo.Service.Sharif.State;

namespace Silo.Service.Sharif.Services;

public sealed class ReaderControlService : IDisposable
{
    private readonly RfidReaderService _reader;
    private readonly ReaderStateStore _state;
    private readonly ILogger<ReaderControlService> _logger;
    private bool _disposed;

    public ReaderControlService(RfidReaderService reader, ReaderStateStore state, ILogger<ReaderControlService> logger)
    {
        _reader = reader ?? throw new ArgumentNullException(nameof(reader));
        _state = state ?? throw new ArgumentNullException(nameof(state));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<bool> ConnectAsync(CancellationToken cancellationToken = default)
    {
        await _state.ReaderLock.WaitAsync(cancellationToken);
        try
        {
            _reader.ConnectUsb();
            _state.SetLastError(null);
            _state.SetConnectionState(true);
            _logger.LogInformation("RFID reader USB connected from UI/Worker.");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "RFID reader USB connect failed.");
            _state.SetLastError($"اتصال به RFID با خطا مواجه شد: {ex.Message}");
            _state.SetConnectionState(false);
            return false;
        }
        finally
        {
            _state.ReaderLock.Release();
        }
    }

    public async Task<bool> DisconnectAsync(CancellationToken cancellationToken = default)
    {
        await _state.ReaderLock.WaitAsync(cancellationToken);
        try
        {
            _reader.Disconnect();
            _state.SetConnectionState(false);
            _state.SetPower(0);
            _logger.LogInformation("RFID reader disconnected from UI/Worker.");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "RFID reader disconnect failed.");
            _state.SetLastError($"قطع اتصال RFID با خطا مواجه شد: {ex.Message}");
            return false;
        }
        finally
        {
            _state.ReaderLock.Release();
        }
    }

    public async Task<bool> StartInventoryAsync(CancellationToken cancellationToken = default)
    {
        await _state.ReaderLock.WaitAsync(cancellationToken);
        try
        {
            _reader.StartInventory();
            _state.SetInventoryRunning(true);
            _state.SetLastError(null);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "RFID inventory start failed.");
            _state.SetLastError($"شروع خواندن RFID با خطا مواجه شد: {ex.Message}");
            _state.SetInventoryRunning(false);
            return false;
        }
        finally
        {
            _state.ReaderLock.Release();
        }
    }

    public async Task<bool> StopInventoryAsync(CancellationToken cancellationToken = default)
    {
        await _state.ReaderLock.WaitAsync(cancellationToken);
        try
        {
            _reader.StopInventory();
            _state.SetInventoryRunning(false);
            _state.SetLastError(null);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "RFID inventory stop failed.");
            _state.SetLastError($"توقف خواندن RFID با خطا مواجه شد: {ex.Message}");
            return false;
        }
        finally
        {
            _state.ReaderLock.Release();
        }
    }

    public async Task<bool> SetPowerAsync(byte power, CancellationToken cancellationToken = default)
    {
        await _state.ReaderLock.WaitAsync(cancellationToken);
        try
        {
            if (power is < 1 or > 30)
            {
                throw new ArgumentOutOfRangeException(nameof(power), "توان آنتن باید بین ۱ تا ۳۰ dBm باشد.");
            }

            _reader.SetPower(0, power);
            _state.SetPower(power);
            _state.SetLastError(null);
            _logger.LogInformation("RFID reader power set to {Power} dBm.", power);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "RFID set power failed.");
            _state.SetLastError($"تنظیم توان RFID با خطا مواجه شد: {ex.Message}");
            return false;
        }
        finally
        {
            _state.ReaderLock.Release();
        }
    }

    public async Task<UHFTAGInfo?> ReadTagAsync(CancellationToken cancellationToken = default)
    {
        await _state.ReaderLock.WaitAsync(cancellationToken);
        try
        {
            return _reader.ReadTag();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "RFID ReadTag failed.");
            _state.SetLastError($"خطا در خواندن برچسب RFID: {ex.Message}");
            return null;
        }
        finally
        {
            _state.ReaderLock.Release();
        }
    }

    public void Dispose()
    {
        if (_disposed) return;
        _state.ReaderLock.Dispose();
        _disposed = true;
    }
}
