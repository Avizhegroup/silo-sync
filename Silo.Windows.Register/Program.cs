using Silo.Windows.Register.Forms;

namespace Silo.Windows.Register;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();
        Application.Run(new MainForm());
    }
}
