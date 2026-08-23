namespace Silo.Application.Features;

public class GetAllProductInStoreVm
{
    public string ProductCode { get; set; }
    public string ProductName { get; set; }
    public string ToDate { get; set; }
    public string TechnicalCode { get; set; }
    public string Qc { get; set; }
    public int Count { get; set; }
    public decimal SumCount { get; set; }
    public string ZoneList { get; set; }
    public string Size { get; set; }
    public string EnterStatus { get; set; }
    public string WarehouseCode { get; set; }
}
