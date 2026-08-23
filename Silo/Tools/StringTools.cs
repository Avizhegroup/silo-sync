using System.Net.Mail;


namespace Silo.Tools;

public static class StringTools
{
    public static List<TelerikDropDownItem> GetIranianPlateAlphabets()
    {
        List<TelerikDropDownItem> items = new();
        items.Add(new() { Name = "", Value = "" });
        items.Add(new() { Name = "الف", Value = "آ" });
        items.Add(new() { Name = "ب", Value = "ب" });
        items.Add(new() { Name = "پ", Value = "پ" });
        items.Add(new() { Name = "ت", Value = "ت" });
        items.Add(new() { Name = "ث", Value = "ث" });
        items.Add(new() { Name = "ج", Value = "ج" });
        items.Add(new() { Name = "چ", Value = "چ" });
        items.Add(new() { Name = "ح", Value = "ح" });
        items.Add(new() { Name = "خ", Value = "خ" });
        items.Add(new() { Name = "د", Value = "د" });
        items.Add(new() { Name = "ذ", Value = "ذ" });
        items.Add(new() { Name = "ر", Value = "ر" });
        items.Add(new() { Name = "ز", Value = "ز" });
        items.Add(new() { Name = "ژ", Value = "ژ" });
        items.Add(new() { Name = "س", Value = "س" });
        items.Add(new() { Name = "ش", Value = "ش" });
        items.Add(new() { Name = "ص", Value = "ص" });
        items.Add(new() { Name = "ض", Value = "ض" });
        items.Add(new() { Name = "ط", Value = "ط" });
        items.Add(new() { Name = "ظ", Value = "ظ" });
        items.Add(new() { Name = "ع", Value = "ع" });
        items.Add(new() { Name = "غ", Value = "غ" });
        items.Add(new() { Name = "ف", Value = "ف" });
        items.Add(new() { Name = "ق", Value = "ق" });
        items.Add(new() { Name = "ک", Value = "ک" });
        items.Add(new() { Name = "گ", Value = "گ" });
        items.Add(new() { Name = "ل", Value = "ل" });
        items.Add(new() { Name = "م", Value = "م" });
        items.Add(new() { Name = "ن", Value = "ن" });
        items.Add(new() { Name = "و", Value = "و" });
        items.Add(new() { Name = "ه", Value = "ه" });
        items.Add(new() { Name = "ی", Value = "ی" });
        return items;
    }

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
