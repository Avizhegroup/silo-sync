using Silo.Application.Features;

namespace Silo.Service.Sharif;

public class Worker : BackgroundService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<Worker> _logger;
    private readonly RfidReaderService _rfid;
    private readonly RfidConnectApiForSharif _api;

    public Worker(ILogger<Worker> logger, RfidReaderService rfid, IConfiguration configuration, RfidConnectApiForSharif api)
    {
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
               
                await _api.SendAsyncObjectByUri<CreateSharifTagVm>(HttpMethod.Post,"Sharif/SendTag",
                 new
                 {
                     EPC = tag.Epc,
                     StationCode = stationCode,
                     GateType = gateType

                 });

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

