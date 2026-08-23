using System.IO.Compression;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.ResponseCompression;
using Silo.Api.Business;
using Silo.Api.Extensions;
using Silo.Api.Services;
using Silo.Application;
using Silo.Application.Api.Contracts;
using Silo.Application.Contracts;
using Silo.Domains;
using Silo.Domains.Android;
using Silo.Identity.Server;
using Silo.Infrastructure.Shared;

namespace Silo.Api;
public static partial class Program
{
    public static IServiceCollection ConfigureServices(this IServiceCollection services
    , IConfiguration configuration)
    {
        services.AddSiloApiVersioning();

        services.AddHttpContextAccessor();

        services.AddIdentityServerServices();

        services.AddSignalR();

        services.AddEndpointsApiExplorer();

        services.AddSiloApiSwagger(configuration);

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

        services.AddResponseCompression(opts =>
        {
            opts.EnableForHttps = true;
            opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { "application/octet-stream" }).ToArray();
            opts.Providers.Add<BrotliCompressionProvider>();
            opts.Providers.Add<GzipCompressionProvider>();
        })
            .Configure<BrotliCompressionProviderOptions>(opt => opt.Level = CompressionLevel.Fastest)
            .Configure<GzipCompressionProviderOptions>(opt => opt.Level = CompressionLevel.Fastest);

        services.AddSiloSerilog(configuration);

        services.AddControllers();

        services.AddApplicationApiServices();

        services.AddSingleton<IDataAccess, SqlDataAccess>();

        services.AddDomainsServices(configuration);

        services.AddDomainsAndroidServices();

        services.AddScoped<IWmsBusiness, WmsBusiness>();

        services.AddScoped<NotificationBusiness>();

        services.AddScoped<DocumentBusiness>();

        services.AddScoped<TruckCrossBusiness>();

        services.AddScoped<ProductBusiness>();

        services.AddScoped<ReportBusiness>();

        services.AddScoped<AppSettingsBusiness>();

        services.AddScoped<ReportFormatBusiness>();

        services.AddScoped<InspectBusiness>();

        services.AddScoped<DocumentLogBusiness>();

        services.AddScoped<CustomerGuaranteeCheckBusiness>();

        services.AddScoped(sp =>
        {
            HttpClient httpClient = new(sp.GetRequiredService<SmsHttpClient>());

            return httpClient;
        });

        services.AddScoped<SmsHttpClient>();

        services.Configure<SaveProductCommandEnabilityCheck>(configuration.GetSection("ProjectConfigs:WmsConfigs:AddProductSettings:PagePropertyEnability"));

        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.SuppressModelStateInvalidFilter = true;
        });

        services.AddAutoMapper(options =>
        {
            options.AddProfile<WmsProfile>();
        });

        ResourceManager.Initialize(configuration);

        services.AddHttpClient<IAiApiClient, AiApiHttpClient>(client =>
        {
            client.BaseAddress = new Uri(configuration["AiApi:BaseUrl"] ?? "http://localhost:5100/");
            client.DefaultRequestHeaders.Add("X-Api-Key", configuration["AiApi:ApiKey"] ?? string.Empty);
        });

        services.AddOptions<RagAiOptions>()
            .Bind(configuration.GetSection(RagAiOptions.SectionName));

        var siloAiOptions = configuration.GetSection(RagAiOptions.SectionName).Get<RagAiOptions>() ?? new RagAiOptions();

        services.AddHttpClient(SiloAiClient.HttpClientName, client =>
        {
            client.BaseAddress = new Uri(siloAiOptions.BaseUrl.HasValue() ? siloAiOptions.BaseUrl : "http://localhost:5100/");
            client.Timeout = TimeSpan.FromSeconds(30);

            if (siloAiOptions.ApiKey.HasValue())
            {
                client.DefaultRequestHeaders.Add("X-Api-Key", siloAiOptions.ApiKey);
            }
        });

        services.AddScoped<ISiloAiClient, SiloAiClient>();

        return services;
    }
}
