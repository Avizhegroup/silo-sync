namespace Silo.Application.Features;

public class SaveMovementActionDirectCommand
{
    public string SourceWarehouseCode { get; set; }

    public string DestinationZoneCode { get; set; }
    public string DestinationWarehouseCode { get; set; }

    public string DirectPlaceActionData { get; set; }
    public string DirectPlaceActionDesc { get; set; }
    public string DocumentId { get; set; }
    public List<string> Serials { get; set; }
    public List<int> LogGateActionIds { get; set; }
    public string GateCode { get; set; }
    public int TruckCrossId { get; set; } = 0;
}
