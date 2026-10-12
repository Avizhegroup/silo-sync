namespace Silo.Queueing;

internal sealed class QueueTaskFaultConsumer(QueueDbContext dbContext)
    : IConsumer<Fault<ExecuteWorkflowTask>>
{
    public async Task Consume(ConsumeContext<Fault<ExecuteWorkflowTask>> context)
    {
        var message = context.Message.Message;
        var workflow = await dbContext.Workflows
            .Include(x => x.Tasks)
            .SingleOrDefaultAsync(x => x.Id == message.WorkflowId, context.CancellationToken);

        if (workflow is null || workflow.Status is QueueWorkflowState.Completed or QueueWorkflowState.Failed)
        {
            return;
        }

        var task = workflow.Tasks.SingleOrDefault(x => x.Index == message.TaskIndex);
        if (task is null || task.Status == QueueTaskState.Completed)
        {
            return;
        }

        var now = DateTimeOffset.UtcNow;
        task.Status = QueueTaskState.Failed;
        task.LeaseUntil = null;
        task.CompletedAt = now;
        task.LastErrorCode = "QUEUE_DELIVERY_FAILED";
        task.LastErrorMessage = "Queue task processing failed after message redelivery.";
        workflow.Status = QueueWorkflowState.Failed;
        workflow.UpdatedAt = now;
        workflow.CompletedAt = null;
        await dbContext.SaveChangesAsync(context.CancellationToken);
    }
}
