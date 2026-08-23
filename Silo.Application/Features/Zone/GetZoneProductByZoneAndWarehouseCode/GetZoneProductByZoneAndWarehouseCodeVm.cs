namespace Silo.Application.Features;

public class GetZoneProductByZoneAndWarehouseCodeVm
{
    public string ZoneCode { get; set; }
    public string RegCode { get; set; }
    public int Count { get; set; }
    public decimal SumCount { get; set; }
    public string FirstDate { get; set; }
    public string LastDate { get; set; }
    public string ProductCode { get; set; }
    public string Qc { get; set; }
    public string WarehouseCode { get; set; }
}
