namespace Silo.Application.Features;

public class GetZoneProductSerialsQuery
{
    public string ZoneCode { get; set; }
    public string ProductCode { get; set; }
    public string ProductSerial { get; set; }
    public string StoreCode { get; set; }
    public string RegCode { get; set; }
    public bool RegCodeLike { get; set; }
}
