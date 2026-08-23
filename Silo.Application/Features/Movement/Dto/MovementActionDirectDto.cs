namespace Silo.Application.Features;

public class MovementActionDirectDto
{
    public int MovementActionId { get; set; }
    public string MovementActionDateTime { get; set; }
    public string SourceWarehouseCode { get; set; }
    public string DestinationZoneCode { get; set; }
    public string DestinationWarehouseCode { get; set; }
    public string MovementActionData { get; set; }
    public string MovementActionDesc { get; set; }
    public string DocumentCode { get; set; }
    public string UserName { get; set; }
    public List<string> GateCodes { get; set; }
    public int TruckCrossId { get; set; } = 0;
}
