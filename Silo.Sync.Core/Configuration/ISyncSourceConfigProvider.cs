using Silo.Sync.Core.Models;

namespace Silo.Sync.Core.Configuration;

public interface ISyncSourceConfigProvider
{
    Task<SyncSourceConfigDto?> GetBySourceKeyAsync(string sourceKey, CancellationToken cancellationToken = default);
}
