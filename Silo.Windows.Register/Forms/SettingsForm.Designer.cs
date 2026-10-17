namespace Silo.Windows.Register.Forms;

partial class SettingsForm
{
    private System.ComponentModel.IContainer? components = null;

    protected override void Dispose(bool disposing)
    {
        if (disposing && components != null)
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        _table = new TableLayoutPanel();
        _serverIpLabel = new Label();
        _serverIp = new TextBox();
        _readerTypeLabel = new Label();
        _readerType = new ComboBox();
        _portLabel = new Label();
        _port = new ComboBox();
        _baudLabel = new Label();
        _baud = new NumericUpDown();
        _powerLabel = new Label();
        _power = new NumericUpDown();
        _ignoreLabel = new Label();
        _ignore = new TextBox();
        _serialLengthLabel = new Label();
        _serialLength = new NumericUpDown();
        _splitterLabel = new Label();
        _splitter = new TextBox();
        _printerLabel = new Label();
        _printer = new ComboBox();
        _socketPortLabel = new Label();
        _socketPort = new NumericUpDown();
        _tokenLabel = new Label();
        _token = new TextBox();
        _originsLabel = new Label();
        _origins = new TextBox();
        _save = new Button();
        _table.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)_baud).BeginInit();
        ((System.ComponentModel.ISupportInitialize)_power).BeginInit();
        ((System.ComponentModel.ISupportInitialize)_serialLength).BeginInit();
        ((System.ComponentModel.ISupportInitialize)_socketPort).BeginInit();
        SuspendLayout();
        // 
        // _table
        // 
        _table.AutoSize = true;
        _table.ColumnCount = 2;
        _table.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 170F));
        _table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        _table.Controls.Add(_serverIpLabel, 0, 0);
        _table.Controls.Add(_serverIp, 1, 0);
        _table.Controls.Add(_readerTypeLabel, 0, 1);
        _table.Controls.Add(_readerType, 1, 1);
        _table.Controls.Add(_portLabel, 0, 2);
        _table.Controls.Add(_port, 1, 2);
        _table.Controls.Add(_baudLabel, 0, 3);
        _table.Controls.Add(_baud, 1, 3);
        _table.Controls.Add(_powerLabel, 0, 4);
        _table.Controls.Add(_power, 1, 4);
        _table.Controls.Add(_ignoreLabel, 0, 5);
        _table.Controls.Add(_ignore, 1, 5);
        _table.Controls.Add(_serialLengthLabel, 0, 6);
        _table.Controls.Add(_serialLength, 1, 6);
        _table.Controls.Add(_splitterLabel, 0, 7);
        _table.Controls.Add(_splitter, 1, 7);
        _table.Controls.Add(_printerLabel, 0, 8);
        _table.Controls.Add(_printer, 1, 8);
        _table.Controls.Add(_socketPortLabel, 0, 9);
        _table.Controls.Add(_socketPort, 1, 9);
        _table.Controls.Add(_tokenLabel, 0, 10);
        _table.Controls.Add(_token, 1, 10);
        _table.Controls.Add(_originsLabel, 0, 11);
        _table.Controls.Add(_origins, 1, 11);
        _table.Dock = DockStyle.Top;
        _table.Location = new Point(0, 0);
        _table.Name = "_table";
        _table.Padding = new Padding(10);
        _table.RowCount = 12;
        _table.RowStyles.Add(new RowStyle());
        _table.RowStyles.Add(new RowStyle());
        _table.RowStyles.Add(new RowStyle());
        _table.RowStyles.Add(new RowStyle());
        _table.RowStyles.Add(new RowStyle());
        _table.RowStyles.Add(new RowStyle());
        _table.RowStyles.Add(new RowStyle());
        _table.RowStyles.Add(new RowStyle());
        _table.RowStyles.Add(new RowStyle());
        _table.RowStyles.Add(new RowStyle());
        _table.RowStyles.Add(new RowStyle());
        _table.RowStyles.Add(new RowStyle());
        _table.Size = new Size(520, 492);
        _table.TabIndex = 0;
        // 
        // _serverIpLabel
        // 
        _serverIpLabel.Anchor = AnchorStyles.Left;
        _serverIpLabel.AutoSize = true;
        _serverIpLabel.Location = new Point(10, 18);
        _serverIpLabel.Margin = new Padding(0, 8, 0, 8);
        _serverIpLabel.Name = "_serverIpLabel";
        _serverIpLabel.Size = new Size(66, 20);
        _serverIpLabel.TabIndex = 0;
        _serverIpLabel.Text = "Server IP";
        // 
        // _serverIp
        // 
        _serverIp.Dock = DockStyle.Fill;
        _serverIp.Location = new Point(183, 13);
        _serverIp.Name = "_serverIp";
        _serverIp.Size = new Size(324, 27);
        _serverIp.TabIndex = 1;
        // 
        // _readerTypeLabel
        // 
        _readerTypeLabel.Anchor = AnchorStyles.Left;
        _readerTypeLabel.AutoSize = true;
        _readerTypeLabel.Location = new Point(10, 54);
        _readerTypeLabel.Margin = new Padding(0, 8, 0, 8);
        _readerTypeLabel.Name = "_readerTypeLabel";
        _readerTypeLabel.Size = new Size(121, 20);
        _readerTypeLabel.TabIndex = 2;
        _readerTypeLabel.Text = "Desk reader type";
        // 
        // _readerType
        // 
        _readerType.Dock = DockStyle.Fill;
        _readerType.DropDownStyle = ComboBoxStyle.DropDownList;
        _readerType.Location = new Point(183, 49);
        _readerType.Name = "_readerType";
        _readerType.Size = new Size(324, 28);
        _readerType.TabIndex = 2;
        // 
        // _portLabel
        // 
        _portLabel.Anchor = AnchorStyles.Left;
        _portLabel.AutoSize = true;
        _portLabel.Location = new Point(10, 90);
        _portLabel.Margin = new Padding(0, 8, 0, 8);
        _portLabel.Name = "_portLabel";
        _portLabel.Size = new Size(88, 20);
        _portLabel.TabIndex = 3;
        _portLabel.Text = "Reader port";
        // 
        // _port
        // 
        _port.Dock = DockStyle.Fill;
        _port.Location = new Point(183, 85);
        _port.Name = "_port";
        _port.Size = new Size(324, 28);
        _port.TabIndex = 3;
        // 
        // _baudLabel
        // 
        _baudLabel.Anchor = AnchorStyles.Left;
        _baudLabel.AutoSize = true;
        _baudLabel.Location = new Point(10, 126);
        _baudLabel.Margin = new Padding(0, 8, 0, 8);
        _baudLabel.Name = "_baudLabel";
        _baudLabel.Size = new Size(124, 20);
        _baudLabel.TabIndex = 4;
        _baudLabel.Text = "Baud rate (Serial)";
        // 
        // _baud
        // 
        _baud.Dock = DockStyle.Fill;
        _baud.Location = new Point(183, 121);
        _baud.Maximum = new decimal(new int[] { 921600, 0, 0, 0 });
        _baud.Minimum = new decimal(new int[] { 1200, 0, 0, 0 });
        _baud.Name = "_baud";
        _baud.Size = new Size(324, 27);
        _baud.TabIndex = 4;
        _baud.Value = new decimal(new int[] { 1200, 0, 0, 0 });
        // 
        // _powerLabel
        // 
        _powerLabel.Anchor = AnchorStyles.Left;
        _powerLabel.AutoSize = true;
        _powerLabel.Location = new Point(10, 162);
        _powerLabel.Margin = new Padding(0, 8, 0, 8);
        _powerLabel.Name = "_powerLabel";
        _powerLabel.Size = new Size(102, 20);
        _powerLabel.TabIndex = 5;
        _powerLabel.Text = "Reader power";
        // 
        // _power
        // 
        _power.Dock = DockStyle.Fill;
        _power.Location = new Point(183, 157);
        _power.Maximum = new decimal(new int[] { 30, 0, 0, 0 });
        _power.Name = "_power";
        _power.Size = new Size(324, 27);
        _power.TabIndex = 5;
        // 
        // _ignoreLabel
        // 
        _ignoreLabel.Anchor = AnchorStyles.Left;
        _ignoreLabel.AutoSize = true;
        _ignoreLabel.Location = new Point(10, 198);
        _ignoreLabel.Margin = new Padding(0, 8, 0, 8);
        _ignoreLabel.Name = "_ignoreLabel";
        _ignoreLabel.Size = new Size(153, 40);
        _ignoreLabel.TabIndex = 6;
        _ignoreLabel.Text = "Ignore EPCs (comma, T64)";
        // 
        // _ignore
        // 
        _ignore.Dock = DockStyle.Fill;
        _ignore.Location = new Point(183, 193);
        _ignore.Name = "_ignore";
        _ignore.Size = new Size(324, 27);
        _ignore.TabIndex = 6;
        // 
        // _serialLengthLabel
        // 
        _serialLengthLabel.Anchor = AnchorStyles.Left;
        _serialLengthLabel.AutoSize = true;
        _serialLengthLabel.Location = new Point(10, 254);
        _serialLengthLabel.Margin = new Padding(0, 8, 0, 8);
        _serialLengthLabel.Name = "_serialLengthLabel";
        _serialLengthLabel.Size = new Size(92, 20);
        _serialLengthLabel.TabIndex = 7;
        _serialLengthLabel.Text = "Serial length";
        // 
        // _serialLength
        // 
        _serialLength.Dock = DockStyle.Fill;
        _serialLength.Location = new Point(183, 249);
        _serialLength.Maximum = new decimal(new int[] { 50, 0, 0, 0 });
        _serialLength.Name = "_serialLength";
        _serialLength.Size = new Size(324, 27);
        _serialLength.TabIndex = 7;
        // 
        // _splitterLabel
        // 
        _splitterLabel.Anchor = AnchorStyles.Left;
        _splitterLabel.AutoSize = true;
        _splitterLabel.Location = new Point(10, 290);
        _splitterLabel.Margin = new Padding(0, 8, 0, 8);
        _splitterLabel.Name = "_splitterLabel";
        _splitterLabel.Size = new Size(114, 20);
        _splitterLabel.TabIndex = 8;
        _splitterLabel.Text = "Barcode splitter";
        // 
        // _splitter
        // 
        _splitter.Dock = DockStyle.Fill;
        _splitter.Location = new Point(183, 285);
        _splitter.MaxLength = 1;
        _splitter.Name = "_splitter";
        _splitter.Size = new Size(324, 27);
        _splitter.TabIndex = 8;
        // 
        // _printerLabel
        // 
        _printerLabel.Anchor = AnchorStyles.Left;
        _printerLabel.AutoSize = true;
        _printerLabel.Location = new Point(10, 326);
        _printerLabel.Margin = new Padding(0, 8, 0, 8);
        _printerLabel.Name = "_printerLabel";
        _printerLabel.Size = new Size(52, 20);
        _printerLabel.TabIndex = 9;
        _printerLabel.Text = "Printer";
        // 
        // _printer
        // 
        _printer.Dock = DockStyle.Fill;
        _printer.Location = new Point(183, 321);
        _printer.Name = "_printer";
        _printer.Size = new Size(324, 28);
        _printer.TabIndex = 9;
        // 
        // _socketPortLabel
        // 
        _socketPortLabel.Anchor = AnchorStyles.Left;
        _socketPortLabel.AutoSize = true;
        _socketPortLabel.Location = new Point(10, 362);
        _socketPortLabel.Margin = new Padding(0, 8, 0, 8);
        _socketPortLabel.Name = "_socketPortLabel";
        _socketPortLabel.Size = new Size(85, 20);
        _socketPortLabel.TabIndex = 10;
        _socketPortLabel.Text = "Socket port";
        // 
        // _socketPort
        // 
        _socketPort.Dock = DockStyle.Fill;
        _socketPort.Location = new Point(183, 357);
        _socketPort.Maximum = new decimal(new int[] { 65535, 0, 0, 0 });
        _socketPort.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
        _socketPort.Name = "_socketPort";
        _socketPort.Size = new Size(324, 27);
        _socketPort.TabIndex = 10;
        _socketPort.Value = new decimal(new int[] { 1, 0, 0, 0 });
        // 
        // _tokenLabel
        // 
        _tokenLabel.Anchor = AnchorStyles.Left;
        _tokenLabel.AutoSize = true;
        _tokenLabel.Location = new Point(10, 398);
        _tokenLabel.Margin = new Padding(0, 8, 0, 8);
        _tokenLabel.Name = "_tokenLabel";
        _tokenLabel.Size = new Size(94, 20);
        _tokenLabel.TabIndex = 11;
        _tokenLabel.Text = "Socket token";
        // 
        // _token
        // 
        _token.Dock = DockStyle.Fill;
        _token.Location = new Point(183, 393);
        _token.Name = "_token";
        _token.Size = new Size(324, 27);
        _token.TabIndex = 11;
        // 
        // _originsLabel
        // 
        _originsLabel.Anchor = AnchorStyles.Left;
        _originsLabel.AutoSize = true;
        _originsLabel.Location = new Point(10, 434);
        _originsLabel.Margin = new Padding(0, 8, 0, 8);
        _originsLabel.Name = "_originsLabel";
        _originsLabel.Size = new Size(119, 40);
        _originsLabel.TabIndex = 12;
        _originsLabel.Text = "Allowed origins (comma, HTTPS)";
        // 
        // _origins
        // 
        _origins.Dock = DockStyle.Fill;
        _origins.Location = new Point(183, 429);
        _origins.Name = "_origins";
        _origins.Size = new Size(324, 27);
        _origins.TabIndex = 12;
        // 
        // _save
        // 
        _save.Dock = DockStyle.Bottom;
        _save.Location = new Point(0, 492);
        _save.Name = "_save";
        _save.Size = new Size(520, 36);
        _save.TabIndex = 13;
        _save.Text = "Save";
        _save.Click += OnSaveClick;
        // 
        // SettingsForm
        // 
        AutoScaleMode = AutoScaleMode.None;
        ClientSize = new Size(520, 528);
        Controls.Add(_table);
        Controls.Add(_save);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "SettingsForm";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Settings";
        _table.ResumeLayout(false);
        _table.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)_baud).EndInit();
        ((System.ComponentModel.ISupportInitialize)_power).EndInit();
        ((System.ComponentModel.ISupportInitialize)_serialLength).EndInit();
        ((System.ComponentModel.ISupportInitialize)_socketPort).EndInit();
        ResumeLayout(false);
        PerformLayout();
    }

    private TableLayoutPanel _table;
    private Label _serverIpLabel;
    private TextBox _serverIp;
    private Label _readerTypeLabel;
    private ComboBox _readerType;
    private Label _portLabel;
    private ComboBox _port;
    private Label _baudLabel;
    private NumericUpDown _baud;
    private Label _powerLabel;
    private NumericUpDown _power;
    private Label _ignoreLabel;
    private TextBox _ignore;
    private Label _serialLengthLabel;
    private NumericUpDown _serialLength;
    private Label _splitterLabel;
    private TextBox _splitter;
    private Label _printerLabel;
    private ComboBox _printer;
    private Label _socketPortLabel;
    private NumericUpDown _socketPort;
    private Label _tokenLabel;
    private TextBox _token;
    private Label _originsLabel;
    private TextBox _origins;
    private Button _save;
}
