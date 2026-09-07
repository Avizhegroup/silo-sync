using Hangfire;
using Hangfire.SqlServer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Silo.Sync.Worker;

/// <summary>
/// Configures Hangfire, Serilog, and hosted services for the sync worker.
/// </summary>
public static class HangfireConfiguration
{
    /// <summary>
    /// Adds sync worker services including Hangfire and Serilog logging.
    /// </summary>
    public static IServiceCollection AddSyncWorker(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddLogging(builder =>
        {
            var loggerConfig = new LoggerConfiguration()
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("Hangfire", LogEventLevel.Warning)
                .Enrich.FromLogContext();

#if DEBUG
            loggerConfig.WriteTo.Console();
#else
            loggerConfig
                .WriteTo.Logger(lc => lc
                    .Filter.ByIncludingOnly(evt => evt.Level >= LogEventLevel.Warning)
                    .WriteTo.File(
                        $"{AppDomain.CurrentDomain.BaseDirectory}/Logs/Exceptions/Log-{DateTime.Now:yyyyMMdd}.log",
                        outputTemplate: "-------------------Exception Begin----------------------{NewLine}Exception Occure Time:{Timestamp:o}{NewLine}Exception Message:{Message}{NewLine}Exception Base:{Exception}{NewLine}-------------------Exception End----------------------{NewLine}"))
                .WriteTo.Logger(lc => lc
                    .Filter.ByIncludingOnly(evt => evt.Level <= LogEventLevel.Information)
                    .WriteTo.File(
                        $"{AppDomain.CurrentDomain.BaseDirectory}/Logs/InfoLogs/Log-{DateTime.Now:yyyyMMdd}.log",
                        outputTemplate: "-------------------Log Begin----------------------{NewLine}Occure Time:{Timestamp:o}{NewLine}Message:{Message}{NewLine}-------------------Log End----------------------{NewLine}"));
#endif
            var logger = loggerConfig.CreateLogger();
            Log.Logger = logger;
            builder.AddSerilog(logger, dispose: true);
        });

        services.AddHangfire((serviceProvider, globalConfiguration) =>
        {
            var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
            var connectionString = configuration.GetConnectionString("SqlDefaultConnectionString")
                ?? throw new InvalidOperationException("SqlDefaultConnectionString is not configured.");

            globalConfiguration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(connectionString, new SqlServerStorageOptions
                {
                    SchemaName = "Hangfire",
                    PrepareSchemaIfNecessary = true,
                    CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                    SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                    QueuePollInterval = TimeSpan.Zero
                })
                .UseSerilogLogProvider();
        });

        services.AddHangfireServer(options =>
        {
            options.WorkerCount = 1;
            options.Queues = new[] { "sync" };
            options.SchedulePollingInterval = TimeSpan.FromSeconds(15);
        });

        services.AddHostedService<SourceScheduleRegistrar>();

        return services;
    }
}
