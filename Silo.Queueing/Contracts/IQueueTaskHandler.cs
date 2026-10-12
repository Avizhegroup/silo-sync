namespace Silo.Queueing;

/// <summary>Application-provided executor for a registered task type.</summary>
public interface IQueueTaskHandler
{
    string TaskType { get; }

    Task<QueueTaskResult> ExecuteAsync(
        QueueTaskContext context,
        CancellationToken cancellationToken);
}
