using Silo.Sync.Core.Models;

namespace Silo.Sync.Core.Retry;

/// <summary>
/// Defines operations for retrying a previously failed sync row immediately.
/// </summary>
public interface IFailedRowRetryService
{
    /// <summary>
    /// Retries a single failed row immediately and returns the upsert result.
    /// </summary>
    Task<UpsertResult> RetryAsync(string sourceKey, string rowKey, CancellationToken cancellationToken = default);
}
