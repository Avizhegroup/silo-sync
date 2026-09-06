using System.IO.Compression;
using Microsoft.AspNetCore.ResponseCompression;
using Silo.Modules.Ai;
using Silo.Profiles;
using Silo.Services;
using Microsoft.FeatureManagement;

namespace Silo;
public static partial class Program
{
    public static void ConfigureServices(this IServiceCollection services
        , IConfiguration configuration)
    {
        services.AddServerSideBlazor()
                .AddCircuitOptions(options => { options.DetailedErrors = true; });

        services.AddRazorPages();

        services.AddControllers();

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

        services.AddSiloClientIdentityServices();

        services.AddTelerikBlazor();

        services.AddAuthenticationCore();

        services.AddInfrastructureWebServices(configuration);

        services.AddScoped<SyncAdminApiClient>();

        services.AddAiModuleServices();

        services.AddOptions();

        services.AddSignalR();

        services.AddAutoMapper(options =>
        {
            options.AddProfile<ApplicationProfile>();
        });

        services.AddFeatureManagement();

        services.AddResponseCompression(opts =>
        {
            opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { "application/octet-stream" });

            opts.Providers.Add<BrotliCompressionProvider>();

            opts.Providers.Add<GzipCompressionProvider>();
        })
            .Configure<BrotliCompressionProviderOptions>(opt => opt.Level = CompressionLevel.Fastest)
            .Configure<GzipCompressionProviderOptions>(opt => opt.Level = CompressionLevel.Fastest);

            }
        }
