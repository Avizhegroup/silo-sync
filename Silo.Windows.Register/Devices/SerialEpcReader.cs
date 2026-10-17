using System.IO.Ports;

namespace Silo.Windows.Register.Devices;

/// <summary>Reads newline-terminated hex EPC lines from a COM port.</summary>
public sealed class SerialEpcReader(string portName, int baudRate) : IEpcReader
{
    private SerialPort? _port;

    public string Name => $"Serial {portName}";
    public bool IsRunning => _port?.IsOpen == true;
    public event EventHandler<string>? EpcRead;

    public void Start()
    {
        if (IsRunning)
        {
            return;
        }

        _port = new SerialPort(portName, baudRate) { NewLine = "\n", ReadTimeout = 500 };
        _port.DataReceived += OnDataReceived;
        _port.Open();
    }

    public void Stop()
    {
        if (_port is null)
        {
            return;
        }

        _port.DataReceived -= OnDataReceived;
        if (_port.IsOpen)
        {
            _port.Close();
        }
        _port.Dispose();
        _port = null;
    }

    private void OnDataReceived(object sender, SerialDataReceivedEventArgs e)
    {
        try
        {
            while (_port is { IsOpen: true, BytesToRead: > 0 })
            {
                var line = _port.ReadLine().Trim();
                if (line.Length > 0)
                {
                    EpcRead?.Invoke(this, line);
                }
            }
        }
        catch (TimeoutException)
        {
        }
        catch (InvalidOperationException)
        {
        }
    }

    public void Dispose() => Stop();
}
