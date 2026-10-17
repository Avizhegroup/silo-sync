namespace Silo.Windows.Register.Devices;

public sealed class SimulatedEpcReader : IEpcReader
{
    private CancellationTokenSource? _cts;

    public string Name => "Simulated";
    public bool IsRunning => _cts is not null;
    public event EventHandler<string>? EpcRead;

    public void Start()
    {
        if (IsRunning)
        {
            return;
        }

        var cts = _cts = new CancellationTokenSource();
        _ = Task.Run(async () =>
        {
            while (!cts.IsCancellationRequested)
            {
                await Task.Delay(2000, cts.Token).ConfigureAwait(false);
                EpcRead?.Invoke(this, Convert.ToHexString(Guid.NewGuid().ToByteArray(), 0, 12));
            }
        }, cts.Token).ContinueWith(_ => { }, TaskScheduler.Default);
    }

    public void Stop()
    {
        _cts?.Cancel();
        _cts?.Dispose();
        _cts = null;
    }

    public void Dispose() => Stop();
}
