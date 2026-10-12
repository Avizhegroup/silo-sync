namespace Silo.Queueing;

/// <summary>Execution context supplied to an application task handler.</summary>
/// <param name="WorkflowId">The parent workflow identifier.</param>
/// <param name="TaskId">The identifier of this task.</param>
/// <param name="TaskIndex">Zero-based position in the original workflow task list.</param>
/// <param name="TaskType">Registered application task type.</param>
/// <param name="Payload">JSON payload supplied by the workflow submitter.</param>
/// <param name="Attempt">One-based execution attempt number.</param>
/// <param name="IdempotencyKey">Stable workflow/task key suitable for downstream idempotency.</param>
public sealed record QueueTaskContext(
    Guid WorkflowId,
    Guid TaskId,
    int TaskIndex,
    string TaskType,
    JsonElement Payload,
    int Attempt,
    string IdempotencyKey);
