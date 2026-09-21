using System.Text.Json;
using Microsoft.Extensions.Options;
using Silo.Application.Features;
using Silo.Service.Sharif.Configuration;
using Silo.Service.Sharif.Services;
using Silo.Service.Sharif.State;

namespace Silo.Service.Sharif;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly RfidConnectApiForSharif _api;
    private readonly ReaderControlService _readerControl;
    private readonly ReaderStateStore _state;
    private readonly IOptionsMonitor<RfidWorkerOptions> _options;
    private readonly List<Silo.Service.Sharif.Dtos.UHFTAGInfo> _pendingTags = new();
    private readonly object _pendingLock = new();
    private Timer? _checkTimer;

    public Worker(
        ILogger<Worker> logger,
        RfidConnectApiForSharif api,
        ReaderControlService readerControl,
        ReaderStateStore state,
        IOptionsMonitor<RfidWorkerOptions> options)
    {
        _logger = logger;
        _api = api;
        _readerControl = readerControl;
        _state = state;
        _options = options;
    }

    public override async Task StartAsync(CancellationToken token)
    {
        try
        {
            //await _readerControl.ConnectAsync(token);

            var power = _options.CurrentValue.ReaderPower;

            //await _readerControl.SetPowerAsync(power, token);

            //_state.SetInventoryRunning(true);

            _logger.LogInformation(
                "RFID worker initialized. Connected={Connected}, Power={Power}, Station={Station}, GateType={GateType}.",
                _state.IsConnected,
                power,
                _options.CurrentValue.StationCode,
                _options.CurrentValue.GateType);

            await base.StartAsync(token);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while starting RFID worker.");
            throw;
        }
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Delay(2000, stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            var options = _options.CurrentValue;
            var idleDelay = options.IdleDelayMilliseconds;

            if (!_state.IsInventoryRunning)
            {
                await Task.Delay(Math.Max(idleDelay, 100), stoppingToken);
                continue;
            }

            try
            {
                var tag = await _readerControl.ReadTagAsync(stoppingToken);

                if (tag != null)
                {
                    _logger.LogInformation("Read tag with {Epc}", tag.Epc);
                    BufferTag(tag, options, stoppingToken);
                }
            }
            catch (OperationCanceledException)
            {
                // Expected when the service is stopping.
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in RFID worker loop.");
                _state.SetLastError($"خطا در حلقه پردازش RFID: {ex.Message}");
            }

            await Task.Delay(500, stoppingToken);
        }

        _logger.LogInformation("RFID tag reading loop exited gracefully.");
    }

    public override async Task StopAsync(CancellationToken token)
    {
        _state.SetInventoryRunning(false);

        lock (_pendingLock)
        {
            _checkTimer?.Dispose();
            _checkTimer = null;
        }

        await _readerControl.StopInventoryAsync(token);
        await _readerControl.DisconnectAsync(token);

        _logger.LogInformation("RFID reader stopped and disconnected successfully.");

        await base.StopAsync(token);
    }

    private void BufferTag(Silo.Service.Sharif.Dtos.UHFTAGInfo tag, RfidWorkerOptions options, CancellationToken stoppingToken)
    {
        lock (_pendingLock)
        {
            _pendingTags.Add(tag);

            var checkTime = TimeSpan.FromSeconds(Math.Max(options.CheckTimeSeconds, 1));
            _checkTimer?.Dispose();
            _checkTimer = new Timer(_ => FlushPendingTags(options, stoppingToken), null, checkTime, Timeout.InfiniteTimeSpan);
        }
    }

    private void FlushPendingTags(RfidWorkerOptions options, CancellationToken stoppingToken)
    {
        List<Silo.Service.Sharif.Dtos.UHFTAGInfo> tagsToSend;

        lock (_pendingLock)
        {
            if (_pendingTags.Count == 0)
            {
                return;
            }

            tagsToSend = new List<Silo.Service.Sharif.Dtos.UHFTAGInfo>(_pendingTags);
            _pendingTags.Clear();

            _checkTimer?.Dispose();
            _checkTimer = null;
        }

        _ = Task.Run(async () =>
        {

            await SendTagAsync(tagsToSend, options, stoppingToken);
        }, CancellationToken.None);
    }

    private async Task SendTagAsync(List<Silo.Service.Sharif.Dtos.UHFTAGInfo> tags, RfidWorkerOptions options, CancellationToken cancellationToken)
    {
        try
        {
            var json = await _api.SendAsyncObjectByUri<CreateSharifTagVm>(
                HttpMethod.Post,
                "Sharif/SendTag",
                new 
                {
                    Epcs = tags.Select(t => t.Epc),
                    options.StationCode,
                    options.GateType
                });

            var succeeded = IsSuccessResponse(json);
            var message = succeeded ? null : (ExtractMessage(json) ?? json);
            _state.RecordApiResult(succeeded, message);

            foreach (var tag in tags)
            {
                _state.AddTag(new TagReadEntry
                {
                    Epc = tag.Epc,
                    ReadUtc = DateTime.UtcNow,
                    StationCode = options.StationCode,
                    GateType = options.GateType,
                    PostSucceeded = succeeded,
                    ErrorMessage = message
                });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send tag to API.");
            _state.RecordApiResult(false, ex.Message);
        }
    }

    private static bool IsSuccessResponse(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return false;
        }

        try
        {
            using var document = JsonDocument.Parse(json);
            if (document.RootElement.TryGetProperty("successful", out var successProperty))
            {
                return successProperty.GetBoolean();
            }

            if (document.RootElement.TryGetProperty("Successful", out var successProp2))
            {
                return successProp2.GetBoolean();
            }
        }
        catch
        {
            // Treat malformed responses as failures.
        }

        return false;
    }

    private static string? ExtractMessage(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return null;
        }

        try
        {
            using var document = JsonDocument.Parse(json);
            if (document.RootElement.TryGetProperty("message", out var message) && message.GetString() is { } m)
            {
                return m;
            }

            if (document.RootElement.TryGetProperty("Message", out var message2) && message2.GetString() is { } m2)
            {
                return m2;
            }
        }
        catch
        {
            // Ignore parse errors; caller can show raw JSON.
        }

        return null;
    }
}

