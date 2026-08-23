using Silo.Application.Features;

namespace Silo.Application.Dto;

public class TruckCargo
{
    public string GateActionId { get; set; }
    public List<CargoProduct> Products { get; set; }
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

public class CargoProduct
{
    public string ProductName { get; set; }
    public string ProductSerial { get; set; }
    public string ProductCode { get; set; }
    public string Regcode { get; set; }
    public string PmCode { get; set; }
    public string PmToStoreCode { get; set; }
    public string PmToZoneCode { get; set; }
    public string PmToStoreTitle { get; set; }
    public string PmToZoneAddress { get; set; }
}
