namespace Silo.Sync.Core.Models;

public sealed record SyncSourceConfigDto
{
    public required string SourceKey { get; init; }
    public string? DisplayName { get; init; }
    public string? SourceType { get; init; }
    public string? ConnectionString { get; init; }
    public string? Command { get; init; }
    public string? FieldKey { get; init; }
    public string? FieldCheck { get; init; }
    public string? FieldOrder { get; init; }
    public int? IntervalSeconds { get; init; }
    public bool IsEnabled { get; init; }
}
