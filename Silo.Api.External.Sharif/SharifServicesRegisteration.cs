using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Silo.Api.External.Sharif.Services;

namespace Silo.Api.External.Sharif;

public static class SharifServicesRegisteration
{
    public static IServiceCollection AddSharifServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        const string baseUrl = "https://api.sharif.edu";
        const int timeoutSeconds = 30;

        var apiKey = configuration["ProjectConfigs:WmsConfigs:ExternalApi:ApiKey"];

        services.AddHttpClient<SharifHttpClientHandler>(client =>
        {
            client.BaseAddress = new Uri(baseUrl);

            client.Timeout = TimeSpan.FromSeconds(timeoutSeconds);

            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

            client.DefaultRequestHeaders.Add("Accept", "application/json");
        });

        services.AddScoped<SharifExternalConnect>();

        return services;
    }
}
