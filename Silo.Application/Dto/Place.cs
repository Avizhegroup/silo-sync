using Silo.Application.Features;

namespace Silo.Application.Dto;
public class PlaceProductRequest
{
    public List<PlaceSerialDto> Serials { get; set; }
    public string SourceZone { get; set; }
    public string SourceWarehouse { get; set; }
    public string DestinationZone { get; set; }
    public string DestinationWarehouse { get; set; }
    public string LogGateActionId { get; set; }
    public string StationCode { get; set; } = "0";
}

public class PlaceSerialDto
{
    public string Serial { get; set; }
    public PlaceType PlaceType { get; set; }
}

public class ProductPlaceFilterRequest
{
    public string SourceWarehouseCode { get; set; }
    public string ProductCode { get; set; }
    public string RegCode { get; set; }
    public string ProductName { get; set; }
}
