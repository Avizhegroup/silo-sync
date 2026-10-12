namespace Silo.Queueing;

internal sealed class QueueHandlerValidationService(IServiceScopeFactory scopeFactory) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var handlers = scope.ServiceProvider.GetServices<IQueueTaskHandler>().ToArray();
        var duplicateType = handlers
            .GroupBy(x => x.TaskType, StringComparer.Ordinal)
            .FirstOrDefault(group => group.Count() > 1);

        if (duplicateType is not null)
        {
            throw new InvalidOperationException(
                $"More than one queue task handler is registered for '{duplicateType.Key}'.");
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
