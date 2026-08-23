namespace Silo.Application.Features;

public class GetMovementAllPlaceProductVm
{

    public string Warehouse { get; set; }
    public string ProductCode { get; set; }
    public string RegCode { get; set; }
    public string ProductName { get; set; }
    public string ProductSerial { get; set; }
    public string ProductType { get; set; }
    public decimal ProductCount { get; set; }
    public string ZoneTitle { get; set; }
    public bool IsChoosed { get; set; }
}
