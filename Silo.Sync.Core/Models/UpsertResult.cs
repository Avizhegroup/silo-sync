namespace Silo.Sync.Core.Models;

public sealed record UpsertResult
{
    public required bool Success { get; init; }
    public string? ErrorCategory { get; init; }
    public string? ErrorMessage { get; init; }
}
