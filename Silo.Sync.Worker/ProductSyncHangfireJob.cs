using Hangfire;
using Microsoft.Data.SqlClient;
using Polly;
using Polly.CircuitBreaker;
using Silo.Sync.Core;

namespace Silo.Sync.Worker;

/// <summary>
/// Hangfire job that runs product sync for a single source with retry and circuit breaker policies.
/// </summary>
public sealed class ProductSyncHangfireJob
{
    private readonly ProductSyncOrchestrator _orchestrator;
    private readonly ILogger<ProductSyncHangfireJob> _logger;
    private readonly AsyncPolicy _executionPolicy;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProductSyncHangfireJob"/> class.
    /// </summary>
    public ProductSyncHangfireJob(ProductSyncOrchestrator orchestrator, ILogger<ProductSyncHangfireJob> logger)
    {
        _orchestrator = orchestrator;
        _logger = logger;

        var retryPolicy = Policy
            .Handle<SqlException>(IsTransient)
            .WaitAndRetryAsync(
                3,
                retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)) + TimeSpan.FromMilliseconds(Random.Shared.Next(0, 1000)),
                (exception, delay, attempt, ctx) =>
                {
                    logger.LogWarning(exception, "Transient source DB failure for sync source. Attempt {Attempt}, retrying in {Delay}s", attempt, delay.TotalSeconds);
                });

        var circuitBreakerPolicy = Policy
            .Handle<SqlException>(IsTransient)
            .CircuitBreakerAsync(
                5,
                TimeSpan.FromMinutes(2),
                onBreak: (exception, duration) => logger.LogError(exception, "Sync circuit breaker opened for {Duration}s", duration.TotalSeconds),
                onReset: () => logger.LogInformation("Sync circuit breaker reset"));

        _executionPolicy = Policy.WrapAsync(retryPolicy, circuitBreakerPolicy);
    }

    /// <summary>
    /// Runs the product sync orchestrator for the specified source key.
    /// </summary>
    [Queue("sync")]
    [AutomaticRetry(Attempts = 0)]
    public async Task RunAsync(string sourceKey, CancellationToken cancellationToken)
    {
        try
        {
            await _executionPolicy.ExecuteAsync(async ct => await _orchestrator.RunAsync(sourceKey, false, ct), cancellationToken);
        }
        catch (BrokenCircuitException ex)
        {
            _logger.LogError(ex, "Circuit open for sync source {SourceKey}; skipping scheduled run", sourceKey);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception in sync job for source {SourceKey}", sourceKey);
            throw;
        }
    }

    private static bool IsTransient(SqlException exception)
    {
        return exception.Number is -2 or 1205 or 121 or 64 or 233 or 10054 or 10060 or 10061
               || (exception.Message is not null && (
                   exception.Message.Contains("timeout", StringComparison.OrdinalIgnoreCase)
                   || exception.Message.Contains("network", StringComparison.OrdinalIgnoreCase)
                   || exception.Message.Contains("connection", StringComparison.OrdinalIgnoreCase)
                   || exception.Message.Contains("reset", StringComparison.OrdinalIgnoreCase)));
    }
}
