using Silo.Application.Features;

namespace Silo.Sync.Core.Models;

/// <summary>
/// Represents the ProductRow record.
/// </summary>
public sealed record ProductRow
{
    /// <summary>
    /// Gets or sets the init.
    /// </summary>
    public required string RowKey { get; init; }
    /// <summary>
    /// Gets or sets the init.
    /// </summary>
    public required DateTime CheckValue { get; init; }
    /// <summary>
    /// Gets or sets the init.
    /// </summary>
    public required string RawPayload { get; init; }
    /// <summary>
    /// Gets or sets the init.
    /// </summary>
    public required SaveProductCommand Command { get; init; }
}
