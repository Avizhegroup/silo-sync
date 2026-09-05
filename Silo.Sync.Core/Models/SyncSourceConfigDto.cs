namespace Silo.Sync.Core.Models;

/// <summary>
/// Represents the SyncSourceConfigDto record.
/// </summary>
public sealed record SyncSourceConfigDto
{
    /// <summary>
    /// Gets or sets the init.
    /// </summary>
    public required string SourceKey { get; init; }
    /// <summary>
    /// Gets or sets the init.
    /// </summary>
    public string? DisplayName { get; init; }
    /// <summary>
    /// Gets or sets the init.
    /// </summary>
    public string? SourceType { get; init; }
    /// <summary>
    /// Gets or sets the init.
    /// </summary>
    public string? ConnectionString { get; init; }
    /// <summary>
    /// Gets or sets the init.
    /// </summary>
    public string? Command { get; init; }
    /// <summary>
    /// Gets or sets the init.
    /// </summary>
    public string? FieldKey { get; init; }
    /// <summary>
    /// Gets or sets the init.
    /// </summary>
    public string? FieldCheck { get; init; }
    /// <summary>
    /// Gets or sets the init.
    /// </summary>
    public string? FieldOrder { get; init; }
    /// <summary>
    /// Gets or sets the init.
    /// </summary>
    public int? IntervalSeconds { get; init; }
    /// <summary>
    /// IsEnabled operation.
    /// </summary>
    public bool IsEnabled { get; init; }
}
