using Silo.Ui.Gate.BLL;

namespace Silo.Ui.Gate;
public partial class frmRegister : Form
{
    public frmRegister()
    {
        InitializeComponent();
    }

    private void frmRegister_Load(object sender, EventArgs e)
    {
        lblAppSerial.Text = "356017";
        lblCpuId.Text = RegisteryClass.GetCPUId();
        txtActivationCode.Focus();
    }

    private void button1_Click(object sender, EventArgs e)
    {
        try
        {
            if (txtActivationCode.Text.Trim().ToLower() == RegisteryClass.CreateActiveCode().ToLower())
            {
                Microsoft.Win32.RegistryKey regkey = Microsoft.Win32.Registry.CurrentUser.CreateSubKey("SOFTWARE\\TDAvizhe\\RFIDConnect_antenna");
                regkey.SetValue("RFIDConnectOfVars", txtActivationCode.Text.Trim().ToLower());
                regkey.Close();
                regkey.Flush();
                MessageBox.Show("برنامه فعال شد");
                this.Hide();
                new FrmMain().ShowDialog();
            }
        }
        catch
        {

        }
    }

    private void button2_Click(object sender, EventArgs e)
    {
        Application.Exit();
    }
}
