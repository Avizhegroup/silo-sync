namespace Silo.Application.Features;
public class CollectPlanDto
{
    public string ProductCode { get; set; }
    public string PackCount { get; set; }
    public string PackCountDescription { get; set; }
    public List<string> ZoneList { get; set; }
    public string FromZoneCode { get; set; }
    public decimal SumValue { get; set; }
    public string Truck { get; set; }
    public List<string> Serials { get; set; }
}
