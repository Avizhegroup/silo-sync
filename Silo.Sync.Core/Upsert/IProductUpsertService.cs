using Silo.Application.Features;
using Silo.Sync.Core.Models;

namespace Silo.Sync.Core.Upsert;

public interface IProductUpsertService
{
    Task<UpsertResult> UpsertOneAsync(SaveProductCommand command, string connectionString, CancellationToken cancellationToken = default);
}
