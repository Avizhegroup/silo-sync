namespace Silo.Queueing;

/// <summary>Application-provided executor for a registered task type.</summary>
public interface IQueueTaskHandler
{
    /// <summary>The unique task type handled by this implementation.</summary>
    string TaskType { get; }

    /// <summary>Executes application-specific task work.</summary>
    Task<QueueTaskResult> ExecuteAsync(
        QueueTaskContext context,
        CancellationToken cancellationToken);
}
