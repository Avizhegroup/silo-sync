namespace Silo.Application.Features;

public class GetAllInspectReportVm
{
    public int InspectId { get; set; }
    public string InspectUsername { get; set; }
    public string ProductName { get; set; }
    public string ProductCode { get; set; }
    public string ProductSerial { get; set; }
    public string Line { get; set; }
    public DateTime ProductionDateTime { get; set; }
    public DateTime DateTime { get; set; }
    public string RegCode { get; set; }
    public InspectResult Result { get; set; }
    public List<InspectElementValues> Values { get; set; }
    public string ProductProperties { get; set; }
}
