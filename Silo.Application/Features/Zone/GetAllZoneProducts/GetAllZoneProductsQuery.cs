namespace Silo.Application.Features;

public class GetAllZoneProductsQuery
{
    public string ProductCode { get; set; }
    public string TechnicalCode { get; set; }
    public string ProductSerial { get; set; }
    public string TagZone { get; set; }
    public string AgeRange { get; set; }
    public bool TechnicalCodeLike { get; set; } = false;
    public bool TagZoneLike { get; set; } = false;
    public string Capacity { get; set; }
    public string MinCapacity { get; set; }
    public string MaxCapacity { get; set; }
    public string ZoneLayer { get; set; }
    public string WarehouseCode { get; set; }
}
