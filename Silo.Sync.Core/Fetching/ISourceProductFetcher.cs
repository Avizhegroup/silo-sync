using Silo.Sync.Core.Models;

namespace Silo.Sync.Core.Fetching;

/// <summary>
/// Defines operations for ISourceProductFetcher.
/// </summary>
public interface ISourceProductFetcher
{
    Task<IReadOnlyList<ProductRow>> FetchAsync(SyncSourceConfigDto source, DateTime? checkpoint, CancellationToken cancellationToken = default);
}
