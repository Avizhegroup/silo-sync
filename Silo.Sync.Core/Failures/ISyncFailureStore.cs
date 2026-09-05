using Silo.Sync.Core.Models;

namespace Silo.Sync.Core.Failures;

/// <summary>
/// Defines operations for ISyncFailureStore.
/// </summary>
public interface ISyncFailureStore
{
    Task RecordFailureAsync(string sourceKey, string rowKey, string? rawPayload, UpsertResult result, int? runLogId, CancellationToken cancellationToken = default);
    Task ResolveFailureAsync(string sourceKey, string rowKey, CancellationToken cancellationToken = default);
    Task<bool> CanAutoRetryAsync(string sourceKey, string rowKey, CancellationToken cancellationToken = default);
}
