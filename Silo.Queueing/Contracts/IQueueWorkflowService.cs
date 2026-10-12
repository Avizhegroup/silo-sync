namespace Silo.Queueing;

/// <summary>Submits and administers durable workflows.</summary>
public interface IQueueWorkflowService
{
    /// <summary>Persists and dispatches a workflow, returning its durable identifier.</summary>
    Task<Guid> SubmitAsync(
        QueueWorkflowRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>Gets workflow and task-level status, or <see langword="null"/> if absent.</summary>
    Task<QueueWorkflowStatus?> GetStatusAsync(
        Guid workflowId,
        CancellationToken cancellationToken = default);

    /// <summary>Resumes a failed workflow at its first incomplete task.</summary>
    Task ResumeAsync(
        Guid workflowId,
        CancellationToken cancellationToken = default);
}
