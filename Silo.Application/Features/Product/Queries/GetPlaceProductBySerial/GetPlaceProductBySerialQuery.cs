namespace Silo.Application.Features;

public class GetPlaceProductBySerialQuery
{
    public List<string> Serials { get; set; } = new();
    public string FromZone { get; set; }
    public string FromWarehouse { get; set; }
    public string DestinationZone { get; set; }
    public string DestinationWarehouse { get; set; }
}