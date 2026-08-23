namespace Silo.Application.Features;

public class TruckCargoDto
{
    public string GateActionId { get; set; }
    public List<CargoProductDto> Products { get; set; }
    public string FromZoneCode { get; set; }
    public string FromZoneTitle { get; set; }
    public string FromWarehouseCode { get; set; }
    public string FromWarehouseTitle { get; set; }
    public string DestinationZoneCode { get; set; }
    public string DestinationZoneTitle { get; set; }
    public string DestinationWarehouseCode { get; set; }
    public string DestinationWarehouseTitle { get; set; }
    public string DriverUsername { get; set; }
    public string DriverUserId { get; set; }
    public string TruckNumber { get; set; }
    public string DestinationAddress { get; set; }
    public string GateTitle { get; set; }
    public string ActionDescription { get; set; }
    public CargoStatus CargoStatus { get; set; }
    public ActionStatus ActionStatus { get; set; }
}
