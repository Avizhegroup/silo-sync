namespace Silo.Queueing;

/// <summary>A workflow to be executed in task order.</summary>
public sealed record QueueWorkflowRequest
{
    /// <summary>Caller-provided key used to make submission idempotent.</summary>
    public required string IdempotencyKey { get; init; }

    /// <summary>Ordered task definitions. Each task type must have a registered handler.</summary>
    public required IReadOnlyList<QueueTaskRequest> Tasks { get; init; }
}

/// <summary>A serializable task definition included in a workflow request.</summary>
public sealed record QueueTaskRequest
{
    /// <summary>Registered handler key; it is never resolved as a CLR type name.</summary>
    public required string TaskType { get; init; }

    /// <summary>JSON payload interpreted and validated by the application handler.</summary>
    public required JsonElement Payload { get; init; }
}
