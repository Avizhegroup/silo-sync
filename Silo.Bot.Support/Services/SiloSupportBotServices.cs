using Silo.Bot.Support.Configuration;
using Silo.Bot.Support.Workers;

namespace Silo.Bot.Support.Services;

public static class SiloSupportBotServices
{
    public static void AddSiloSupportBotServices(this IServiceCollection services, IConfiguration configuration)
    {
       services.AddOptions<BaleOptions>()
    .Bind(configuration.GetSection(BaleOptions.SectionName));

        services.AddOptions<RagAiOptions>()
            .Bind(configuration.GetSection(RagAiOptions.SectionName));

        var baleOptions = configuration.GetSection(BaleOptions.SectionName).Get<BaleOptions>();
        var siloAiOptions = configuration.GetSection(RagAiOptions.SectionName).Get<RagAiOptions>();

        services.AddHttpClient(BaleBotClient.HttpClientName, client =>
        {
            var baseUrl = $"{baleOptions.ApiBaseUrl}/bot{baleOptions.BotToken}/";
            client.BaseAddress = new Uri(baseUrl);
            client.Timeout = TimeSpan.FromSeconds(baleOptions.LongPollTimeoutSeconds + 30);
        });

        services.AddHttpClient(SiloAiClient.HttpClientName, client =>
        {
            client.BaseAddress = new Uri(siloAiOptions.BaseUrl);
            client.Timeout = TimeSpan.FromSeconds(30);
            client.DefaultRequestHeaders.Add("X-Api-Key", siloAiOptions.ApiKey);
        });

        services.AddSingleton<BaleBotClient>();
        services.AddSingleton<ISiloAiClient, SiloAiClient>();
        services.AddHostedService<BaleBotWorker>();
    }
}
