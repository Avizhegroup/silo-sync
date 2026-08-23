namespace Silo.Ui.Gate;
public partial class frmConnectionSetting : Form
{
    public frmConnectionSetting()
    {
        InitializeComponent();
    }

    private void frmConnectionSetting_Load(object sender, EventArgs e)
    {
        string[] Temp = Properties.Settings.Default.ServerIp.Split(':');
        txtServerIp.Text = Temp[0];
        txtServerPort.Text = (Temp.Length > 1) ? Temp[1] : "80";

    }

    private void btnSave_Click(object sender, EventArgs e)
    {
        Properties.Settings.Default.ServerIp = txtServerIp.Text+":"+ txtServerPort.Text;
        Properties.Settings.Default.Save();
        MessageBox.Show("ثبت تنظیمات با موفقیت انجام شد");
        Application.Restart();

    }

    private void txtServerPort_Leave(object sender, EventArgs e)
    {
        if (txtServerPort.Text.Trim() == "")
            txtServerPort.Text = "80";
    }

    private void button1_Click(object sender, EventArgs e)
    {
        this.Hide();
        new FrmMain().ShowDialog();
    }
}
