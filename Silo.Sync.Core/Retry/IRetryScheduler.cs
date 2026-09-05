namespace Silo.Sync.Core.Retry;

public interface IRetryScheduler
{
    DateTime GetNextAttemptTime(int attemptCount, DateTime? lastAttemptAt = null);
}
