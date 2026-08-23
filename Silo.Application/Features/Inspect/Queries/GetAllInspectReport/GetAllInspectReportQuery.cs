namespace Silo.Application.Features;
public class GetAllInspectReportQuery
{
    public string FromDate { get; set; }
    public string ToDate { get; set; }
    public string ProductCode { get; set; }
    public string ProductSerial { get; set; }
    public string UserId { get; set; }
    public string Line { get; set; }
    public string RegCode { get; set; }
    public int InspectResult { get; set; } = -1;
    public List<InspectElementValues> ElementFilters { get; set; } = new();
    public List<ChoosableKeyValue> DynamicFilters { get; set; } = new();
}
