using Microsoft.Extensions.FileProviders;
using Silo.Infrastructure.Shared;
using Silo.Jobs.Win.Services;

namespace Silo.Jobs.Win;
public static class Startup
{
    public static void ConfigureServices( this IServiceCollection services
        , IConfiguration configuration)
    {
        services.AddSiloSerilogForWindowsServices(configuration);

        services.AddSingleton<Api>();

        services.AddHostedService<HostedServiceManager>();
    }

    public static void Configure(this IApplicationBuilder app)
    {
        if (!Directory.Exists($"{AppDomain.CurrentDomain.BaseDirectory}/Files"))
        {
            Directory.CreateDirectory($"{AppDomain.CurrentDomain.BaseDirectory}/Files");
        }

        app.UseFileServer(new FileServerOptions
        {
            FileProvider = new PhysicalFileProvider($"{AppDomain.CurrentDomain.BaseDirectory}/Files"),
            RequestPath = "/Files",
            EnableDefaultFiles = true
        });

        app.UseRouting();

        app.UseDeveloperExceptionPage();
    }
}
