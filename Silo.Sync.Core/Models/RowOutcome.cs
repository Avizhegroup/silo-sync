namespace Silo.Sync.Core.Models;

/// <summary>
/// Defines possible values for RowOutcome.
/// </summary>
public enum RowOutcome
{
    Succeeded,
    Failed,
    SkippedNullKey,
    DuplicateKey
}
