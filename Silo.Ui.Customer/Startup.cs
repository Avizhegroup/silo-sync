using System.IO.Compression;
using Microsoft.AspNetCore.ResponseCompression;
using Silo.Identity.Client;
using Silo.Infrastructure.Shared;
using Silo.Ui.Customer.Services;

namespace Silo.Ui.Customer;
public static class Startup
{
    public static void ConfigureServices(this IServiceCollection services
        , IConfiguration configuration)
    {
        services.AddServerSideBlazor()
                .AddCircuitOptions(options => { options.DetailedErrors = true; });

        services.AddRazorPages();

        services.AddCors(options =>
        {
            options.AddPolicy("OpenCors", builder =>
            {
                builder
                    .AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });

        services.AddSiloSerilog(configuration);

        // Register HttpClient and custom SiloApiClient instead of RfidConnectApi
        services.AddHttpClient<ISiloApiClient, SiloApiClient>();

        // Register custom identity services that use ISiloApiClient
        services.AddScoped<AuthenticationStateProvider, SiloAuthenticationStateProvider>();
        services.AddScoped(sp => (SiloAuthenticationStateProvider)sp.GetRequiredService<AuthenticationStateProvider>());
        services.AddScoped<IAuthenticationService, CustomerAuthenticationService>();
        services.AddScoped<IClaimManager, CustomerClaimManager>();

        services.AddTelerikBlazor();

        services.AddAuthenticationCore();

        services.AddOptions();

        services.AddSignalR();

        services.AddResponseCompression(opts =>
        {
            opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { "application/octet-stream" });

            opts.Providers.Add<BrotliCompressionProvider>();

            opts.Providers.Add<GzipCompressionProvider>();
        })
            .Configure<BrotliCompressionProviderOptions>(opt => opt.Level = CompressionLevel.Fastest)
            .Configure<GzipCompressionProviderOptions>(opt => opt.Level = CompressionLevel.Fastest);
    }

    public static void Configure(this WebApplication app)
    {
#if DEBUG
        app.UseDeveloperExceptionPage();
#else
        app.UseExceptionHandler("/Home/Error");
#endif

        app.UseStaticFiles();

        app.UseRouting();

        app.UseCors("OpenCors");

        app.UseResponseCompression();

        app.MapDefaultControllerRoute();

        app.MapBlazorHub();

        app.MapFallbackToPage("/_Host");
    }
}
