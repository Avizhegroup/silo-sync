using System.Diagnostics;

namespace Silo.Ui.Gate.Tools;
public static class ExceptionLogger
{
    public static void WriteExceptionLogs(Exception ex)
    {
        try
        {
#if DEBUG
            Debugger.Break();
#else
            var fileName = "Exceptions_log";
          
            if (!Directory.Exists(Application.StartupPath + "\\Exceptions"))
            {
                Directory.CreateDirectory(Application.StartupPath + "\\Exceptions");
            }

            var filePath = Application.StartupPath + "\\Exceptions\\Exception" + fileName + ".log";

            using StreamWriter file = new (filePath, true);

            file.WriteLine("-------------------Exception Begin----------------------");
            file.WriteLine("Exception Message: {0}", ex.Message);
            file.WriteLine("Exception Base: {0}", ex.GetBaseException());
            file.WriteLine("Exception Raise Time : {0} \n {1}", DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString());
            file.WriteLine("-------------------Exception End----------------------");
            file.WriteLine();
           
            file.Close();
#endif
        }
        catch (Exception ex2) 
        {
        }
    }
}
