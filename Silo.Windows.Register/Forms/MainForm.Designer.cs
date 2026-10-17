namespace Silo.Windows.Register.Forms;

partial class MainForm
{
    private System.ComponentModel.IContainer? components = null;
    private Label _serverStatus;
    private Label _clientStatus;
    private Label _readerStatus;
    private Button _toggleServer;
    private Button _toggleReader;
    private Button _settingsButton;
    private TextBox _log;
    private FlowLayoutPanel _buttonsPanel;
    private FlowLayoutPanel _statusPanel;

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
        _serverStatus = new Label();
        _clientStatus = new Label();
        _readerStatus = new Label();
        _toggleServer = new Button();
        _toggleReader = new Button();
        _settingsButton = new Button();
        _log = new TextBox();
        _buttonsPanel = new FlowLayoutPanel();
        _statusPanel = new FlowLayoutPanel();
        _buttonsPanel.SuspendLayout();
        _statusPanel.SuspendLayout();
        SuspendLayout();
        //
        // _serverStatus
        //
        _serverStatus.AutoSize = true;
        _serverStatus.Name = "_serverStatus";
        _serverStatus.Text = "Server: stopped";
        //
        // _clientStatus
        //
        _clientStatus.AutoSize = true;
        _clientStatus.Name = "_clientStatus";
        _clientStatus.Text = "Browser: not connected";
        //
        // _readerStatus
        //
        _readerStatus.AutoSize = true;
        _readerStatus.Name = "_readerStatus";
        _readerStatus.Text = "Reader: stopped";
        //
        // _toggleServer
        //
        _toggleServer.AutoSize = true;
        _toggleServer.Name = "_toggleServer";
        _toggleServer.Text = "Start server";
        _toggleServer.Click += OnToggleServerClick;
        //
        // _toggleReader
        //
        _toggleReader.AutoSize = true;
        _toggleReader.Name = "_toggleReader";
        _toggleReader.Text = "Start reader";
        _toggleReader.Click += OnToggleReaderClick;
        //
        // _settingsButton
        //
        _settingsButton.AutoSize = true;
        _settingsButton.Name = "_settingsButton";
        _settingsButton.Text = "Settings";
        _settingsButton.Click += OnSettingsClick;
        //
        // _log
        //
        _log.Dock = DockStyle.Fill;
        _log.Multiline = true;
        _log.Name = "_log";
        _log.ReadOnly = true;
        _log.ScrollBars = ScrollBars.Vertical;
        //
        // _buttonsPanel
        //
        _buttonsPanel.AutoSize = true;
        _buttonsPanel.Dock = DockStyle.Top;
        _buttonsPanel.Name = "_buttonsPanel";
        _buttonsPanel.Padding = new Padding(8);
        _buttonsPanel.Controls.Add(_toggleServer);
        _buttonsPanel.Controls.Add(_toggleReader);
        _buttonsPanel.Controls.Add(_settingsButton);
        //
        // _statusPanel
        //
        _statusPanel.AutoSize = true;
        _statusPanel.Dock = DockStyle.Top;
        _statusPanel.FlowDirection = FlowDirection.TopDown;
        _statusPanel.Name = "_statusPanel";
        _statusPanel.Padding = new Padding(8);
        _statusPanel.Controls.Add(_serverStatus);
        _statusPanel.Controls.Add(_clientStatus);
        _statusPanel.Controls.Add(_readerStatus);
        //
        // MainForm
        //
        AutoScaleMode = AutoScaleMode.None;
        ClientSize = new Size(720, 460);
        Controls.Add(_log);
        Controls.Add(_statusPanel);
        Controls.Add(_buttonsPanel);
        KeyPreview = true;
        Name = "MainForm";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Silo Register - Device Host";
        _buttonsPanel.ResumeLayout(false);
        _buttonsPanel.PerformLayout();
        _statusPanel.ResumeLayout(false);
        _statusPanel.PerformLayout();
        ResumeLayout(false);
        PerformLayout();
    }
}
