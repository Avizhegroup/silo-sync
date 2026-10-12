namespace Silo.Queueing;

/// <summary>Outcome returned by an application task handler.</summary>
public sealed record QueueTaskResult
{
    /// <summary>Whether the handler completed the requested operation.</summary>
    public bool Succeeded { get; init; }

    /// <summary>Whether a failure may succeed if attempted again later.</summary>
    public bool Retryable { get; init; }

    /// <summary>Optional safe machine-readable failure code.</summary>
    public string? ErrorCode { get; init; }

    /// <summary>Optional safe-to-display failure detail; do not include credentials or secrets.</summary>
    public string? SafeErrorMessage { get; init; }

    /// <summary>Creates a successful result.</summary>
    public static QueueTaskResult Success() => new() { Succeeded = true };

    /// <summary>Creates a failed result and indicates whether it should be retried.</summary>
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
