namespace Silo.Queueing;

/// <summary>Persistent workflow lifecycle values.</summary>
public enum QueueWorkflowState
{
    /// <summary>Accepted and waiting to execute.</summary>
    Pending,
    /// <summary>A task is currently being executed.</summary>
    Running,
    /// <summary>A retryable failure is waiting for its scheduled retry.</summary>
    Retrying,
    /// <summary>Every task succeeded.</summary>
    Completed,
    /// <summary>A permanent failure or retry exhaustion stopped the workflow.</summary>
    Failed
}

/// <summary>Persistent task lifecycle values.</summary>
public enum QueueTaskState
{
    /// <summary>Not yet executed.</summary>
    Pending,
    /// <summary>Currently executing.</summary>
    Running,
    /// <summary>Waiting for a scheduled retry.</summary>
    Retrying,
    /// <summary>Executed successfully.</summary>
    Completed,
    /// <summary>Execution stopped after a permanent failure or retry exhaustion.</summary>
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
