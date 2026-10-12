namespace Silo.Queueing;

/// <summary>Execution context supplied to an application task handler.</summary>
public sealed record QueueTaskContext(
    Guid WorkflowId,
    Guid TaskId,
    int TaskIndex,
    string TaskType,
    JsonElement Payload,
    int Attempt,
    string IdempotencyKey);
