using Hangfire;
using Hangfire.Storage;
using Silo.Sync.Core.Configuration;

namespace Silo.Sync.Worker;

/// <summary>
/// Hosted service that registers and refreshes Hangfire recurring jobs from sync source configuration.
/// </summary>
public sealed class SourceScheduleRegistrar : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<SourceScheduleRegistrar> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="SourceScheduleRegistrar"/> class.
    /// </summary>
    public SourceScheduleRegistrar(IServiceProvider serviceProvider, ILogger<SourceScheduleRegistrar> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    /// <summary>
    /// Periodically refreshes recurring jobs from the database.
    /// </summary>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await RefreshSchedulesAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to refresh sync source schedules");
            }

            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
        }
    }

    private async Task RefreshSchedulesAsync(CancellationToken cancellationToken)
    {
        await using var scope = _serviceProvider.CreateAsyncScope();
        var configProvider = scope.ServiceProvider.GetRequiredService<ISyncSourceConfigProvider>();
        var recurringJobManager = scope.ServiceProvider.GetRequiredService<IRecurringJobManager>();
        var storage = scope.ServiceProvider.GetRequiredService<JobStorage>();

        var configs = await configProvider.GetAllEnabledAsync(cancellationToken);
        var activeJobIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var config in configs)
        {
            if (string.IsNullOrWhiteSpace(config.SourceKey))
            {
                continue;
            }

            var jobId = $"sync-{config.SourceKey}";
            activeJobIds.Add(jobId);
            var cron = ToCronExpression(config.IntervalSeconds ?? 60);

            recurringJobManager.AddOrUpdate<ProductSyncHangfireJob>(
                jobId,
                job => job.RunAsync(config.SourceKey, CancellationToken.None),
                cron,
                new RecurringJobOptions
                {
                    TimeZone = TimeZoneInfo.Local
                });

            _logger.LogInformation("Registered/updated recurring sync job {JobId} with cron {Cron}", jobId, cron);
        }

        RemoveStaleJobs(storage, recurringJobManager, activeJobIds);
    }

    private static void RemoveStaleJobs(JobStorage storage, IRecurringJobManager recurringJobManager, HashSet<string> activeJobIds)
    {
        using var connection = storage.GetConnection();
        var existingJobs = connection.GetRecurringJobs();

        foreach (var job in existingJobs.Where(j => j.Id.StartsWith("sync-", StringComparison.OrdinalIgnoreCase) && !activeJobIds.Contains(j.Id)))
        {
            recurringJobManager.RemoveIfExists(job.Id);
        }
    }

    private static string ToCronExpression(int intervalSeconds)
    {
        if (intervalSeconds < 60)
        {
            return "* * * * *";
        }

        var minutes = intervalSeconds / 60;
        if (minutes >= 60)
        {
            var hours = minutes / 60;
            if (hours >= 24)
            {
                return "0 0 * * *";
            }

            return hours == 1 ? "0 * * * *" : $"0 */{hours} * * *";
        }

        return minutes == 1 ? "* * * * *" : $"*/{minutes} * * * *";
    }
}
