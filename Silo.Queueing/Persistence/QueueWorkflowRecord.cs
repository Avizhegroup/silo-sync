namespace Silo.Queueing;

internal sealed class QueueWorkflowRecord
{
    public Guid Id { get; set; }
    public required string IdempotencyKeyHash { get; set; }
    public required string RequestHash { get; set; }
    public QueueWorkflowState Status { get; set; }
    public int CurrentTaskIndex { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public DateTimeOffset? StartedAt { get; set; }
    public DateTimeOffset? CompletedAt { get; set; }
    public byte[] RowVersion { get; set; } = [];
    public List<QueueTaskRecord> Tasks { get; set; } = [];
}

internal sealed class QueueTaskRecord
{
    public Guid Id { get; set; }
    public Guid WorkflowId { get; set; }
    public QueueWorkflowRecord Workflow { get; set; } = null!;
    public int Index { get; set; }
    public required string TaskType { get; set; }
    public required string Payload { get; set; }
    public QueueTaskState Status { get; set; }
    public int AttemptCount { get; set; }
    public DateTimeOffset? StartedAt { get; set; }
    public DateTimeOffset? CompletedAt { get; set; }
    public DateTimeOffset? LeaseUntil { get; set; }
    public DateTimeOffset? NextAttemptAt { get; set; }
    public string? LastErrorCode { get; set; }
    public string? LastErrorMessage { get; set; }
    public byte[] RowVersion { get; set; } = [];
}
