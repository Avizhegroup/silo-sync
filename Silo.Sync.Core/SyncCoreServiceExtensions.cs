using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Silo.Domains.Services;
using Silo.Sync.Core.Encryption;
using Silo.Sync.Core.Checkpoints;
using Silo.Sync.Core.Configuration;
using Silo.Sync.Core.Encryption;
using Silo.Sync.Core.Failures;
using Silo.Sync.Core.Fetching;
using Silo.Sync.Core.Retry;
using Silo.Sync.Core.RunLogging;
using Silo.Sync.Core.Upsert;

namespace Silo.Sync.Core;

/// <summary>
/// Registers Silo.Sync.Core services with the dependency injection container.
/// </summary>
public static class SyncCoreServiceExtensions
{
    /// <summary>
    /// Adds Silo.Sync.Core services and the WmsApiContext to the service collection.
    /// </summary>
    public static IServiceCollection AddSyncCoreServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<WmsApiContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("SqlDefaultConnectionString")));

        services.AddSingleton<ISyncConnectionStringProtector>(sp =>
        {
            var protector = sp.GetRequiredService<IDataProtectionProvider>()
                .CreateProtector("Silo.Sync.Core.ConnectionString");
            return new DataProtectionSyncConnectionStringProtector(protector);
        });

        services.AddScoped<ISyncSourceConfigProvider, SyncSourceConfigProvider>();
        services.AddScoped<ISyncCheckpointStore, SyncCheckpointStore>();
        services.AddScoped<ISyncRunLogger, SyncRunLogger>();
        services.AddScoped<ISyncFailureStore, SyncFailureStore>();
        services.AddScoped<IRetryScheduler, RetryScheduler>();
        services.AddScoped<ISourceProductFetcher, SourceProductFetcher>();
        services.AddScoped<IProductUpsertService, ProductUpsertService>();
        services.AddScoped<ProductSyncOrchestrator>();
        services.AddScoped<IFailedRowRetryService, FailedRowRetryService>();

        return services;
    }
}
