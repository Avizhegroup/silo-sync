namespace Silo.Sync.Core.Models;

public enum RowOutcome
{
    Succeeded,
    Failed,
    SkippedNullKey,
    DuplicateKey
}
