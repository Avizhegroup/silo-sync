namespace Silo.Sync.Core.Checkpoints;

/// <summary>
/// Defines operations for ISyncCheckpointStore.
/// </summary>
public interface ISyncCheckpointStore
{
    Task<DateTime?> GetCheckpointAsync(string sourceKey, CancellationToken cancellationToken = default);
    Task AdvanceCheckpointAsync(string sourceKey, DateTime checkpointValue, CancellationToken cancellationToken = default);
}
