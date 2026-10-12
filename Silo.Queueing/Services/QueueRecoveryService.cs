namespace Silo.Queueing;

internal sealed class QueueRecoveryService(
    IServiceScopeFactory scopeFactory,
    IBus bus,
    IOptions<CompanyQueueingOptions> options,
    ILogger<QueueRecoveryService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(15));
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await DispatchRecoverableTasksAsync(stoppingToken);
                await timer.WaitForNextTickAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Queue recovery scan failed.");
                await Task.Delay(TimeSpan.FromSeconds(15), stoppingToken);
            }
        }
    }

    private async Task DispatchRecoverableTasksAsync(CancellationToken cancellationToken)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<QueueDbContext>();
        var now = DateTimeOffset.UtcNow;
        var recoverable = await dbContext.Tasks
            .AsNoTracking()
            .Where(task =>
                task.Workflow.CurrentTaskIndex == task.Index &&
                (task.Status == QueueTaskState.Running && task.LeaseUntil <= now ||
                 task.Status == QueueTaskState.Retrying && task.NextAttemptAt <= now))
            .OrderBy(task => task.Workflow.CreatedAt)
            .Take(options.Value.Processing.ConcurrentWorkflows * 10)
            .Select(task => new ExecuteWorkflowTask(task.WorkflowId, task.Index))
            .ToArrayAsync(cancellationToken);

        foreach (var message in recoverable)
        {
            await bus.Publish(message, cancellationToken);
        }
    }
}
