namespace Silo.Windows.Register.Devices;

public interface IEpcReader : IDisposable
{
    string Name { get; }
    bool IsRunning { get; }
    event EventHandler<string>? EpcRead;
    void Start();
    void Stop();
}
