using System.Collections.Concurrent;

namespace Silo.Service.Sharif.State;

public sealed class ReaderStateStore
{
    private readonly ConcurrentQueue<TagReadEntry> _tags = new();
    private readonly Lock _stateLock = new();
    private const int MaxTags = 500;

    public SemaphoreSlim ReaderLock { get; } = new(1, 1);

    public bool IsConnected { get; private set; }

    public bool IsInventoryRunning { get; private set; }

    public byte CurrentPower { get; private set; }

    public string? LastError { get; private set; }

    public long SuccessCount { get; private set; }

    public long FailureCount { get; private set; }

    public string? LastApiError { get; private set; }

    public DateTime? LastApiCallUtc { get; private set; }

    public int TagCount => _tags.Count;

    public IReadOnlyList<TagReadEntry> Tags
    {
        get
        {
            lock (_stateLock)
            {
                return [.. _tags];
            }
        }
    }

    public event Action? OnChange;

    public void SetConnectionState(bool connected)
    {
        lock (_stateLock)
        {
            IsConnected = connected;
            if (!connected)
            {
                IsInventoryRunning = false;
            }
        }

        NotifyChanged();
    }

    public void SetInventoryRunning(bool running)
    {
        lock (_stateLock)
        {
            IsInventoryRunning = running;
        }

        NotifyChanged();
    }

    public void SetPower(byte power)
    {
        lock (_stateLock)
        {
            CurrentPower = power;
        }

        NotifyChanged();
    }

    public void SetLastError(string? error)
    {
        lock (_stateLock)
        {
            LastError = error;
        }

        NotifyChanged();
    }

    public void RecordApiResult(bool succeeded, string? error = null)
    {
        lock (_stateLock)
        {
            if (succeeded)
            {
                SuccessCount++;
                LastApiError = null;
            }
            else
            {
                FailureCount++;
                LastApiError = error;
            }

            LastApiCallUtc = DateTime.UtcNow;
        }

        NotifyChanged();
    }

    public void AddTag(TagReadEntry entry)
    {
        while (_tags.Count >= MaxTags && _tags.TryDequeue(out _))
        {
        }

        _tags.Enqueue(entry);
        NotifyChanged();
    }

    public void ClearTags()
    {
        while (_tags.TryDequeue(out _))
        {
        }

        NotifyChanged();
    }

    private void NotifyChanged()
    {
        OnChange?.Invoke();
    }
}
