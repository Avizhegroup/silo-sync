namespace Silo.Infrastructure.Web;
public static class InfrastructureWebServices
{
    public static IServiceCollection AddInfrastructureWebServices(this IServiceCollection services
        , IConfiguration configuration)
    {
        services.AddMemoryCache(options =>
        {
            options.SizeLimit = 1024 * 100; // 100MB worth of cache entries (adjust as needed)
        });

        services.AddScoped<RfidConnectApi>();

        services.AddScoped(sp =>
        {
            HttpClient httpClient = new(sp.GetRequiredService<RfidConnectApi>());

            return httpClient;
        });

        services.AddScoped<IFormalDataCache, FormalDataCache>();

        services.AddScoped<IExport, ExportService>();

        services.AddScoped<IExcelExport, ExcelExport>();

        //services.AddScoped(typeof(IPdfExporter), typeof(PdfExporter));

        return services;
    }
}
