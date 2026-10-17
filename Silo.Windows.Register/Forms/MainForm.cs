using System.ComponentModel;
using Silo.Windows.Register.Config;
using Silo.Windows.Register.Devices;
using Silo.Windows.Register.Printing;
using Silo.Windows.Register.Server;

namespace Silo.Windows.Register.Forms;

public sealed partial class MainForm : Form
{
    private readonly SettingsStore _store = new();
    private RegisterSettings _settings = null!;
    private ReaderManager _reader = null!;
    private DeviceSocketServer _server = null!;
    private BarcodeWedgeBuffer _wedge = null!;
    private PrintCommandHandler _printing = null!;

    public MainForm()
    {
        InitializeComponent();

        if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
        {
            return;
        }

        _settings = _store.Load();
        (_reader, _server, _wedge) = Build();

        KeyPress += (_, e) => _wedge.Append(e.KeyChar);
        FormClosing += OnMainFormClosing;
    }

    private (ReaderManager, DeviceSocketServer, BarcodeWedgeBuffer) Build()
    {
        var reader = new ReaderManager(_settings);
        var server = new DeviceSocketServer(_settings);
        var wedge = new BarcodeWedgeBuffer(_settings);

        reader.EpcRead += async (_, epc) => await ForwardAsync("epc", epc);
        reader.StatusChanged += (_, message) => UiPost(() =>
        {
            _readerStatus.Text = $"Reader: {(reader.IsRunning ? "running" : "stopped")}";
            _toggleReader.Text = reader.IsRunning ? "Stop reader" : "Start reader";
            AppendLog(message);
        });
        _printing = new PrintCommandHandler(_settings, new LabelFormatStore(), server, message => UiPost(() => AppendLog(message)));
        wedge.BarcodeRead += async (_, barcode) => await ForwardAsync("barcode", barcode);
        server.Log += (_, message) => UiPost(() => AppendLog(message));
        server.ClientConnectionChanged += (_, connected) => UiPost(() =>
            _clientStatus.Text = connected ? "Browser: connected" : "Browser: not connected");
        server.CommandReceived += (_, command) => UiPost(() => HandleCommand(command));
        return (reader, server, wedge);
    }

    private async Task ForwardAsync(string type, string value)
    {
        UiPost(() => AppendLog($"{type}: {value}"));
        try
        {
            await _server.SendAsync(type, value);
        }
        catch (Exception exception) when (exception is System.Net.WebSockets.WebSocketException or ObjectDisposedException)
        {
        }
    }

    private void HandleCommand(SocketCommand command)
    {
        try
        {
            switch (command.Type.ToLowerInvariant())
            {
                case "startreader":
                    _reader.Start(command.Mode == 2 ? ReadMode.Continuous : ReadMode.Single);
                    break;
                case "stopreader":
                    _reader.Stop();
                    break;
                case "print":
                case "listprinters":
                case "listformats":
                case "printerstatus":
                case "printformat":
                    _ = _printing.HandleAsync(command);
                    break;
                case "ping":
                    _ = _server.SendAsync("pong", string.Empty);
                    break;
                default:
                    AppendLog($"Unknown command: {command.Type}");
                    break;
            }
        }
        catch (Exception exception)
        {
            AppendLog($"Command '{command.Type}' failed: {exception.Message}");
            _ = _server.SendAsync("error", exception.Message);
        }
    }

    private void OnToggleServerClick(object? sender, EventArgs e)
    {
        try
        {
            if (_server.IsRunning)
            {
                _ = _server.StopAsync();
            }
            else
            {
                _server.Start();
            }
            _serverStatus.Text = $"Server: {(_server.IsRunning ? $"listening on 127.0.0.1:{_settings.SocketPort}" : "stopped")}";
            _toggleServer.Text = _server.IsRunning ? "Stop server" : "Start server";
        }
        catch (InvalidOperationException exception)
        {
            MessageBox.Show(this, exception.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }

    private void OnToggleReaderClick(object? sender, EventArgs e)
    {
        try
        {
            if (_reader.IsRunning)
            {
                _reader.Stop();
            }
            else
            {
                _reader.Start(ReadMode.Continuous);
            }
        }
        catch (Exception exception)
        {
            MessageBox.Show(this, exception.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }

    private async void OnSettingsClick(object? sender, EventArgs e)
    {
        using var dialog = new SettingsForm(_store, _settings);
        if (dialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        await _server.DisposeAsync();
        _reader.Dispose();
        _wedge.Dispose();
        _settings = _store.Load();
        (_reader, _server, _wedge) = Build();
        _serverStatus.Text = "Server: stopped";
        _toggleServer.Text = "Start server";
        _toggleReader.Text = "Start reader";
        AppendLog("Settings applied");
    }

    private async void OnMainFormClosing(object? sender, FormClosingEventArgs e)
    {
        _reader.Dispose();
        _wedge.Dispose();
        await _server.DisposeAsync();
    }

    private void UiPost(Action action)
    {
        if (IsDisposed)
        {
            return;
        }

        if (InvokeRequired)
        {
            BeginInvoke(action);
        }
        else
        {
            action();
        }
    }

    private void AppendLog(string message) => _log.AppendText($"{DateTime.Now:HH:mm:ss}  {message}{Environment.NewLine}");
}
