namespace Silo.Application.Features;

public class GetAllInventoryQuery
{
    public string FromDate { get; set; }
    public string ToDate { get; set; }
    public string FromTime { get; set; }
    public string ToTime { get; set; }
    public string User { get; set; }
    public string Desc { get; set; }
    public string Place { get; set; }
    public string Code { get; set; }
    public string WarehouseCode { get; set; }
    public string TechnicalCode { get; set; }
    public string ProductCode { get; set; }
}
