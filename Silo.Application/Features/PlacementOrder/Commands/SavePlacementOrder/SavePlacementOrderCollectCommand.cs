namespace Silo.Application.Features;
public class SavePlacementOrderCollectCommand
{
    public List<CollectPlanDto> CollectPlans { get; set; }
    public string ProductLine { get; set; }
    public string ProductShift { get; set; }
    public string StoreCode { get; set; }
    public string POCode { get; set; }
    public string FromZoneCode { get; set; }
    public string Type { get; set; }
    public string DocumentKey { get; set; }
    public string DocumentType { get; set; }
}
