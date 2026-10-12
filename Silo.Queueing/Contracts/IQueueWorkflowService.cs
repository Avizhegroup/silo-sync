namespace Silo.Queueing;

/// <summary>Submits and administers durable workflows.</summary>
public interface IQueueWorkflowService
{
    Task<Guid> SubmitAsync(
        QueueWorkflowRequest request,
        CancellationToken cancellationToken = default);

    Task<QueueWorkflowStatus?> GetStatusAsync(
        Guid workflowId,
        CancellationToken cancellationToken = default);

    Task ResumeAsync(
        Guid workflowId,
        CancellationToken cancellationToken = default);
}
