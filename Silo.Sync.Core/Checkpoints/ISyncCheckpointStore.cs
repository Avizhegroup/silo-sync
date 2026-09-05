namespace Silo.Sync.Core.Checkpoints;

public interface ISyncCheckpointStore
{
    Task<DateTime?> GetCheckpointAsync(string sourceKey, CancellationToken cancellationToken = default);
    Task AdvanceCheckpointAsync(string sourceKey, DateTime checkpointValue, CancellationToken cancellationToken = default);
}
