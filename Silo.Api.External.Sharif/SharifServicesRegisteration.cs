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
        var baseUrl = configuration["ProjectConfigs:WmsConfigs:ExternalApi:BaseUrl"];

        if (string.IsNullOrWhiteSpace(baseUrl))
            throw new InvalidOperationException(
                "Sharif External API BaseUrl is not configured.");

        var apiKey = configuration["ProjectConfigs:WmsConfigs:ExternalApi:ApiKey"];

        services.AddHttpClient<SharifHttpClientHandler>(client =>
        {
            client.BaseAddress = new Uri(baseUrl);
            client.Timeout = TimeSpan.FromSeconds(30);

            client.DefaultRequestHeaders.Add("Accept", "application/json");

            if (!string.IsNullOrWhiteSpace(apiKey))
            {
                client.DefaultRequestHeaders.Add(
                    "Authorization",
                    $"Bearer {apiKey}");
            }
        });

        services.AddScoped<SharifExternalConnect>();

        return services;
    }
}
