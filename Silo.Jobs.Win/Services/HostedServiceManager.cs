namespace Silo.Jobs.Win.Services;
public class HostedServiceManager : BackgroundService
{
    private readonly ILogger<HostedServiceManager> logger;
    private readonly Api api;
    private readonly IConfiguration configuration;

    public HostedServiceManager(ILogger<HostedServiceManager> logger
        , Api api
        , IConfiguration configuration)
    {
        this.logger = logger;
        this.api = api;
        this.configuration = configuration;
    }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation($"Service started at: {DateTime.Now}");

        await base.StartAsync(cancellationToken);
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation($"Service stopped at: {DateTime.Now}");

        await base.StopAsync(new());
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(TimeSpan.FromMinutes(1));

            await Task.WhenAll(RunDocumentReplace()
                             , RunSendNotification()
                             , RunEventNotification()
                             , RunScheduleNotification()
                             , RunGetProduct());
        }
    }

    public async Task RunDocumentReplace()
    {
        if (configuration["Modes"].NotContains("Document"))
        {
            return;
        }

        logger.LogInformation($"RunDocumentReplace: {DateTime.Now}");

        var typeStr = configuration["Document:Type"];

        bool result = false;

        if (typeStr.Contains(","))
        {
            var types = typeStr.Split(',');

            foreach (var type in types)
            {
                result = (await api.PostAsync<bool>("SCheckCustomerDocumentAdd"
                                  , new KeyValuePair<string, object>("doc", type))).Value;

                logger.LogInformation($"RunDocumentReplace result:{result} , type:{type}");
            }
        }
        else
        {
            result = (await api.PostAsync<bool>("SCheckCustomerDocumentAdd"
             , new KeyValuePair<string, object>("doc", typeStr))).Value;

            logger.LogInformation($"RunDocumentReplace result: {result}");
        }
    }

    public async Task RunScheduleNotification()
    {
        if (configuration["Modes"].NotContains("Schedule"))
        {
            return;
        }

        logger.LogInformation($"RunScheduleNotification: {DateTime.Now}");

        bool result = (api.PostAsyncByUri<bool>("wms/Notification", "SGetScheduleNotifications")).GetAwaiter().GetResult().Value;

        logger.LogInformation($"RunScheduleNotification result: {result}");
    }

    public async Task RunEventNotification()
    {
        if (configuration["Modes"].NotContains("Event"))
        {
            return;
        }

        logger.LogInformation($"RunEventNotification: {DateTime.Now}");

        bool result = (api.PostAsyncByUri<bool>("wms/Notification", "SGetEventNotification")).GetAwaiter().GetResult().Value;

        logger.LogInformation($"RunEventNotification result: {result}");
    }

    public async Task RunSendNotification()
    {
        if (configuration["Modes"].NotContains("Queue"))
        {
            return;
        }

        logger.LogInformation($"RunSendNotification: {DateTime.Now}");

        bool result = (api.PostAsyncByUri<bool>("wms/Notification", "SSendQueueNotifications")).GetAwaiter().GetResult().Value;

        logger.LogInformation($"RunSendNotification result: {result}");
    }

    public async Task RunGetProduct()
    {
        if (configuration["Modes"].NotContains("Product"))
        {
            return;
        }

        logger.LogInformation($"RunGetProduct: {DateTime.Now}");

        var typeStr = configuration["Product:Type"];

        bool result = false;

        if (typeStr.Contains(","))
        {
            var types = typeStr.Split(',');

            foreach (var type in types)
            {
                result = (await api.PostAsync<bool>("SCheckCustomerProducts"
                    , new KeyValuePair<string, object>("key", type))).Value;

                logger.LogInformation($"RunGetProduct result:{result} , type:{type}");
            }
        }
        else
        {
            result = (await api.PostAsync<bool>("SCheckCustomerProducts"
                   , new KeyValuePair<string, object>("key", typeStr))).Value;

            logger.LogInformation($"RunGetProduct result: {result}");
        }
    }
}
