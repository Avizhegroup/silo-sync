using Microsoft.EntityFrameworkCore;
using Silo.Domains.Entities;
using Silo.Domains.Services;

namespace Silo.Sync.Core.Checkpoints;

/// <summary>
/// Represents the SyncCheckpointStore class.
/// </summary>
public sealed class SyncCheckpointStore(WmsApiContext context) : ISyncCheckpointStore
{
    /// <summary>
    /// GetCheckpointAsync operation.
    /// </summary>
    public async Task<DateTime?> GetCheckpointAsync(string sourceKey, CancellationToken cancellationToken = default)
    {
        var entity = await context.SyncCheckpoints
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.SourceKey == sourceKey, cancellationToken);

        return entity?.LastCheckpointValue;
    }

    /// <summary>
    /// AdvanceCheckpointAsync operation.
    /// </summary>
    public async Task AdvanceCheckpointAsync(string sourceKey, DateTime checkpointValue, CancellationToken cancellationToken = default)
    {
        var entity = await context.SyncCheckpoints
            .FirstOrDefaultAsync(x => x.SourceKey == sourceKey, cancellationToken);

        if (entity is null)
        {
            entity = new SyncCheckpoint
            {
                SourceKey = sourceKey,
                LastCheckpointValue = checkpointValue,
                UpdatedDate = DateTime.UtcNow
            };
            context.SyncCheckpoints.Add(entity);
        }
        else if (entity.LastCheckpointValue is null || checkpointValue > entity.LastCheckpointValue)
        {
            entity.LastCheckpointValue = checkpointValue;
            entity.UpdatedDate = DateTime.UtcNow;
        }

        await context.SaveChangesAsync(cancellationToken);
    }
}
