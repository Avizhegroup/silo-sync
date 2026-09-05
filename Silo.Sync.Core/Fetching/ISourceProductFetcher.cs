using Silo.Sync.Core.Models;

namespace Silo.Sync.Core.Fetching;

public interface ISourceProductFetcher
{
    Task<IReadOnlyList<ProductRow>> FetchAsync(SyncSourceConfigDto source, DateTime? checkpoint, CancellationToken cancellationToken = default);
}
