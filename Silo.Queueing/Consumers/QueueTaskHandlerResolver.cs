namespace Silo.Queueing;

internal sealed class QueueTaskHandlerResolver(IEnumerable<IQueueTaskHandler> handlers)
{
    private readonly IReadOnlyDictionary<string, IQueueTaskHandler> _handlers =
        CreateHandlerMap(handlers);

    public void EnsureRegistered(string taskType)
    {
        if (!_handlers.ContainsKey(taskType))
        {
            throw new InvalidOperationException($"No queue task handler is registered for '{taskType}'.");
        }
    }

    public IQueueTaskHandler Resolve(string taskType)
    {
        return _handlers.TryGetValue(taskType, out var handler)
            ? handler
            : throw new InvalidOperationException($"No queue task handler is registered for '{taskType}'.");
    }

    private static IReadOnlyDictionary<string, IQueueTaskHandler> CreateHandlerMap(
        IEnumerable<IQueueTaskHandler> handlers)
    {
        var map = new Dictionary<string, IQueueTaskHandler>(StringComparer.Ordinal);
        foreach (var handler in handlers)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(handler.TaskType);
            if (!map.TryAdd(handler.TaskType, handler))
            {
                throw new InvalidOperationException(
                    $"More than one queue task handler is registered for '{handler.TaskType}'.");
            }
        }

        return map;
    }
}
