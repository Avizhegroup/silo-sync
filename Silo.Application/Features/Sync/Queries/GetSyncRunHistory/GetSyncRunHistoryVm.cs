namespace Silo.Application.Features;

public class GetSyncRunHistoryVm
{
    public long Id { get; set; }
    public string? SourceKey { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? FinishedAt { get; set; }
    public int? RowsFetched { get; set; }
    public int? RowsSucceeded { get; set; }
    public int? RowsFailed { get; set; }
    public string? Status { get; set; }
    public string? ErrorSummary { get; set; }
}
