namespace Silo.Application.Features;

public class GetOpenSyncFailuresVm
{
    public long Id { get; set; }
    public long? SyncRunLogId { get; set; }
    public string? SourceKey { get; set; }
    public string? RowKey { get; set; }
    public string? ErrorCategory { get; set; }
    public string? ErrorMessage { get; set; }
    public int AttemptCount { get; set; }
    public DateTime? LastAttemptAt { get; set; }
    public DateTime? NextAttemptAt { get; set; }
    public string? Status { get; set; }
    public DateTime? ResolvedDate { get; set; }
}
