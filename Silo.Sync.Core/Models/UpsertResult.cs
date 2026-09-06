namespace Silo.Sync.Core.Models;

/// <summary>
/// Represents the UpsertResult record.
/// </summary>
public sealed record UpsertResult
{
    /// <summary>
    /// Gets or sets the init.
    /// </summary>
    public required bool Success { get; init; }
    /// <summary>
    /// Gets or sets the init.
    /// </summary>
    public string? ErrorCategory { get; init; }
    /// <summary>
    /// Gets or sets the init.
    /// </summary>
    public string? ErrorMessage { get; init; }
}
