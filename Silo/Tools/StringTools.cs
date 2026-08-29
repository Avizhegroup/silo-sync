using System.Net.Mail;


namespace Silo.Tools;

public static class StringTools
{
    public static bool IsValidEmail(string emailaddress)
    {
        try
        {
            MailAddress m = new MailAddress(emailaddress);

            return true;
        }
        catch (FormatException)
        {
            return false;
        }
    }
}
