namespace Silo.Queueing;

/// <summary>Outcome returned by an application task handler.</summary>
public sealed record QueueTaskResult
{
    public bool Succeeded { get; init; }
    public bool Retryable { get; init; }
    public string? ErrorCode { get; init; }
    public string? SafeErrorMessage { get; init; }

    public static QueueTaskResult Success() => new() { Succeeded = true };

    public static QueueTaskResult Failure(
        string safeErrorMessage,
        bool retryable,
        string? errorCode = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(safeErrorMessage);
        return new QueueTaskResult
        {
            Retryable = retryable,
            ErrorCode = errorCode,
            SafeErrorMessage = safeErrorMessage
        };
    }
}
