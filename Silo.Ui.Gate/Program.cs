namespace Silo.Ui.Gate;

internal static class Program
{
    [STAThread]
    static void Main()
    {
        if (Properties.Settings.Default.ServerIp == "")
        {
            Application.Run(new frmConnectionSetting());
        }
        else
        {
            Application.Run(new FrmMain());
        }
    }
}
