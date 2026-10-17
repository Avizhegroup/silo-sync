using Silo.Windows.Register.Config;
using Silo.Windows.Register.Devices;

namespace Silo.Windows.Register.Forms;

public sealed class SettingsForm : Form
{
    private readonly SettingsStore _store;
    private readonly RegisterSettings _settings;
    private readonly ComboBox _readerType = new() { DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly ComboBox _port = new() { DropDownStyle = ComboBoxStyle.DropDown };
    private readonly NumericUpDown _baud = new() { Minimum = 1200, Maximum = 921600 };
    private readonly NumericUpDown _power = new() { Minimum = 0, Maximum = 30 };
    private readonly TextBox _ignore = new();
    private readonly NumericUpDown _serialLength = new() { Minimum = 0, Maximum = 50 };
    private readonly TextBox _splitter = new() { MaxLength = 1 };
    private readonly ComboBox _printer = new() { DropDownStyle = ComboBoxStyle.DropDown };
    private readonly NumericUpDown _socketPort = new() { Minimum = 1, Maximum = 65535 };
    private readonly TextBox _token = new();
    private readonly TextBox _origins = new();
    private readonly TextBox _serverIp = new();

    public SettingsForm(SettingsStore store, RegisterSettings settings)
    {
        _store = store;
        _settings = settings;
        Text = "Settings";
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        StartPosition = FormStartPosition.CenterParent;
        ClientSize = new Size(520, 480);

        _readerType.DataSource = Enum.GetValues<DeskReaderType>();
        _port.Items.AddRange(System.IO.Ports.SerialPort.GetPortNames());
        _printer.Items.AddRange(PrinterService.GetInstalledPrinters().ToArray());

        var table = new TableLayoutPanel { Dock = DockStyle.Top, AutoSize = true, ColumnCount = 2, Padding = new Padding(10) };
        table.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 170));
        table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        AddRow(table, "Server IP", _serverIp);
        AddRow(table, "Desk reader type", _readerType);
        AddRow(table, "Reader port", _port);
        AddRow(table, "Baud rate (Serial)", _baud);
        AddRow(table, "Reader power", _power);
        AddRow(table, "Ignore EPCs (comma, T64)", _ignore);
        AddRow(table, "Serial length", _serialLength);
        AddRow(table, "Barcode splitter", _splitter);
        AddRow(table, "Printer", _printer);
        AddRow(table, "Socket port", _socketPort);
        AddRow(table, "Socket token", _token);
        AddRow(table, "Allowed origins (comma, HTTPS)", _origins);

        var save = new Button { Text = "Save", DialogResult = DialogResult.None, Dock = DockStyle.Bottom, Height = 36 };
        save.Click += OnSaveClick;
        Controls.Add(table);
        Controls.Add(save);

        Load += (_, _) => BindFromSettings();
    }

    private static void AddRow(TableLayoutPanel table, string caption, Control control)
    {
        control.Dock = DockStyle.Fill;
        table.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        table.Controls.Add(new Label { Text = caption, AutoSize = true, Anchor = AnchorStyles.Left, Margin = new Padding(0, 8, 0, 8) });
        table.Controls.Add(control);
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
