using Silo.Application.Features;

namespace Silo.Sync.Core.Models;

public sealed record ProductRow
{
    public required string RowKey { get; init; }
    public required DateTime CheckValue { get; init; }
    public required string RawPayload { get; init; }
    public required SaveProductCommand Command { get; init; }
}
