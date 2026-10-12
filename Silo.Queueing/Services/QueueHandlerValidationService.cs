namespace Silo.Queueing;

internal sealed class QueueHandlerValidationService(IServiceScopeFactory scopeFactory) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        _ = scope.ServiceProvider.GetRequiredService<QueueTaskHandlerResolver>();
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
