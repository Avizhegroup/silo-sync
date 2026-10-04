using Silo.Api.External.Sharif.Models;
using Silo.Api.External.Sharif.Services;
using Silo.Application.Features;

namespace Silo.Service.Sharif;

public class Worker : BackgroundService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<Worker> _logger;
    private readonly RfidReaderService _rfid;
    private readonly RfidConnectApiForSharif _api;
    private readonly IServiceScopeFactory _scopeFactory;

    public Worker(ILogger<Worker> logger, RfidReaderService rfid, IConfiguration configuration, RfidConnectApiForSharif api, IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
        _rfid = rfid;
        _configuration = configuration;
        _api = api;
    }

    public override async Task StartAsync(CancellationToken token)
    {
        _rfid.ConnectUsb();

        var powerStr = _configuration["RfidWorker:ReaderPower"];

        if (!byte.TryParse(powerStr, out var powerValue))
            throw new InvalidOperationException($"Invalid Power Configuration: {powerStr}");

        _rfid.SetPower(0, powerValue);

        _logger.LogInformation("RFID reader initialized successfully. USB connection established, power set to {Power}, inventory started.");

        await base.StartAsync(token);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var idleDelayStr = _configuration["RfidWorker:IdleDelayMilliseconds"];
        int idleDelay = int.Parse(idleDelayStr!);
        var stationCode = _configuration["RfidWorker:StationCode"];
        var gateType = _configuration["RfidWorker:GateType"];

        await Task.Delay(2000);

        while (!stoppingToken.IsCancellationRequested)
        {
            _rfid.StartInventory();

            var tag = _rfid.ReadTag();

            if (tag != null)
            {
                _logger.LogInformation($"Read tag with {tag.Epc}");
               
                var result = await _api.SendAsyncObjectByUri<CreateSharifTagVm>(HttpMethod.Post,"Sharif/SendTag",
                 new
                 {
                     Epcs = new List<string> { tag.Epc },
                     StationCode = stationCode,
                     GateType = gateType
                 });

                var snapshotRequest = new RfidSnapshotInnerRequest
                {
                    KioskId = stationCode,
                    ReaderId = "1",
                    SequenceNo = result.Value.OperationCode,
                    CapturedAt = DateTime.Now,
                    Tags = new List<RfidSnapshotTag>
                    {
                        new RfidSnapshotTag { uid = tag.Epc }
                    }
                };

                using var scope = _scopeFactory.CreateScope();
                var sharifExternalConnect = scope.ServiceProvider.GetRequiredService<SharifExternalConnect>();
                RfidSnapShotOutterRequest otterRequest = new()
                {
                    Data = snapshotRequest
                };
                await sharifExternalConnect.SendRegisterTagToExternalApi(snapshotRequest, stoppingToken);
            }

            await Task.Delay(500);
        }

        _logger.LogInformation("RFID tag reading loop exited gracefully.");
    }

    public override async Task StopAsync(CancellationToken token)
    {
        _rfid.StopInventory();

        _rfid.Disconnect();

        _logger.LogInformation("RFID reader stopped and disconnected successfully.");

        await base.StopAsync(token);
    }
}

