namespace Silo.Queueing;

public enum QueueWorkflowState
{
    Pending,
    Running,
    Retrying,
    Completed,
    Failed
}

public enum QueueTaskState
{
    Pending,
    Running,
    Retrying,
    Completed,
    Failed
}

/// <summary>Workflow state and task-level execution details.</summary>
public sealed record QueueWorkflowStatus(
    Guid WorkflowId,
    QueueWorkflowState Status,
    int CurrentTaskIndex,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt,
    DateTimeOffset? StartedAt,
    DateTimeOffset? CompletedAt,
    IReadOnlyList<QueueTaskStatus> Tasks);

/// <summary>Persisted status for one workflow task.</summary>
public sealed record QueueTaskStatus(
    Guid TaskId,
    int Index,
    string TaskType,
    QueueTaskState Status,
    int AttemptCount,
    DateTimeOffset? StartedAt,
    DateTimeOffset? CompletedAt,
    string? LastErrorCode,
    string? LastErrorMessage);
