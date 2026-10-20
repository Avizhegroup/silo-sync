using Silo.Windows.Register.Forms;

namespace Silo.Windows.Register;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();
        Localizer.Load();
        Application.Run(new MainForm());
    }
}
