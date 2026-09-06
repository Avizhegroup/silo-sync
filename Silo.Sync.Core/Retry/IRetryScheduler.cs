namespace Silo.Sync.Core.Retry;

/// <summary>
/// Defines operations for IRetryScheduler.
/// </summary>
public interface IRetryScheduler
{
    DateTime GetNextAttemptTime(int attemptCount, DateTime? lastAttemptAt = null);
}
