using TextResources = Silo.Shared.TextResources;
using Silo.Windows.Register.Config;
using Silo.Windows.Register.Devices;

namespace Silo.Windows.Register.Forms;

public sealed partial class SettingsForm : Form
{
    private SettingsStore _store = null!;
    private RegisterSettings _settings = null!;

    // Required by the Visual Studio designer.
    public SettingsForm()
    {
        InitializeComponent();
        Text = Localizer.Text(TextResources.APP_StringKeys_Settings, "Settings");
        _save.Text = Localizer.Text(TextResources.APP_StringKeys_Save, "Save");
        _serverIpLabel.Text = Localizer.Text(TextResources.APP_StringKeys_Server_Address, "Server IP");
        _readerTypeLabel.Text = Localizer.Text(TextResources.APP_StringKeys_Rfid_DeviceType, "Desk reader type");
        _serialLengthLabel.Text = Localizer.Text(TextResources.APP_StringKeys_Chars_Count, "Serial length");
        _splitterLabel.Text = Localizer.Text(TextResources.APP_StringKeys_Separator_Phrase, "Barcode splitter");
        Load += OnSettingsFormLoad;
    }

    public SettingsForm(SettingsStore store, RegisterSettings settings) : this()
    {
        _store = store;
        _settings = settings;
    }

    private void OnSettingsFormLoad(object? sender, EventArgs e)
    {
        if (_settings is null)
        {
            return;
        }

        _readerType.DataSource = Enum.GetValues<DeskReaderType>();
        _port.Items.AddRange(System.IO.Ports.SerialPort.GetPortNames());
        _printer.Items.AddRange(PrinterService.GetInstalledPrinters().ToArray());
        BindFromSettings();
    }

    private void BindFromSettings()
    {
        _serverIp.Text = _settings.ServerIP;
        _readerType.SelectedItem = _settings.DeskReaderType;
        _port.Text = _settings.DeskUHFReaderPort;
        _baud.Value = Math.Clamp(_settings.BaudRate, (int)_baud.Minimum, (int)_baud.Maximum);
        _power.Value = Math.Clamp(_settings.ReaderPower, 0, 30);
        _ignore.Text = _settings.IgnoreEpcList;
        _serialLength.Value = Math.Clamp(_settings.SerialLength, 0, 50);
        _splitter.Text = _settings.BarcodeSpliter;
        _printer.Text = _settings.PrinterModel;
        _socketPort.Value = Math.Clamp(_settings.SocketPort, 1, 65535);
        _token.Text = _settings.SocketToken;
        _origins.Text = string.Join(",", _settings.AllowedOrigins);
    }

    private void OnSaveClick(object? sender, EventArgs e)
    {
        var origins = _origins.Text.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList();
        if (string.IsNullOrWhiteSpace(_token.Text) || origins.Any(o => !Uri.TryCreate(o, UriKind.Absolute, out var uri) || uri.Scheme != Uri.UriSchemeHttps))
        {
            MessageBox.Show(this, "A token and HTTPS-only origins are required.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        _settings.ServerIP = _serverIp.Text.Trim();
        _settings.DeskReaderType = (DeskReaderType)_readerType.SelectedItem!;
        _settings.DeskUHFReaderPort = _port.Text.Trim();
        _settings.BaudRate = (int)_baud.Value;
        _settings.ReaderPower = (int)_power.Value;
        _settings.IgnoreEpcList = _ignore.Text.Trim();
        _settings.SerialLength = (int)_serialLength.Value;
        _settings.BarcodeSpliter = _splitter.Text;
        _settings.PrinterModel = _printer.Text.Trim();
        _settings.SocketPort = (int)_socketPort.Value;
        _settings.SocketToken = _token.Text.Trim();
        _settings.AllowedOrigins = origins;
        _store.Save(_settings);
        DialogResult = DialogResult.OK;
    }
}
