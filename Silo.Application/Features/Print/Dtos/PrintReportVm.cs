namespace Silo.Application.Features;
public class PrintReportVm
{
    public string? ProductSerial { get; set; }
    public string? ProductCode { get; set; }
    public string? ProductName { get; set; }
    public string? ProductProperties { get; set; }
    public string? ProductPropertiesValue { get => JsonTools.GetFormattedInARowTextFromJson(ProductProperties); }
    public DateTime? DateTime { get; set; }
    public string? Date { get => PersianCalendarTools.GregorianToPersian(DateTime); }
    public string? Time { get => DateTime?.ToString("HH:mm"); }
    public string? RegisterUser { get; set; }
    public int? PrintFlag { get; set; }
    public int? RegisterFlag { get; set; }
    public decimal? ProductCount { get; set; }
    public string? ProductRegCode { get; set; }
    public string? SoftDeleteUser { get; set; }
    public string? SoftDeleteDate { get; set; }
}
