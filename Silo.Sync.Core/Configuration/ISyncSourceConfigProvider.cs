using Silo.Sync.Core.Models;

namespace Silo.Sync.Core.Configuration;

/// <summary>
/// Defines operations for ISyncSourceConfigProvider.
/// </summary>
public interface ISyncSourceConfigProvider
{
    Task<SyncSourceConfigDto?> GetBySourceKeyAsync(string sourceKey, CancellationToken cancellationToken = default);
}
