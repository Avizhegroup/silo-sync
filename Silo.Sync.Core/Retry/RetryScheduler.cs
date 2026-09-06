namespace Silo.Sync.Core.Retry;

/// <summary>
/// Represents the RetryScheduler class.
/// </summary>
public sealed class RetryScheduler : IRetryScheduler
{
    /// <summary>
    /// GetNextAttemptTime operation.
    /// </summary>
    public DateTime GetNextAttemptTime(int attemptCount, DateTime? lastAttemptAt = null)
    {
        var baseTime = lastAttemptAt ?? DateTime.UtcNow;

        var delay = attemptCount switch
        {
            <= 0 => TimeSpan.Zero,
            1 => TimeSpan.Zero,
            2 => TimeSpan.FromMinutes(5),
            3 => TimeSpan.FromMinutes(30),
            _ => TimeSpan.FromMinutes(60)
        };

        return baseTime + delay;
    }
}
