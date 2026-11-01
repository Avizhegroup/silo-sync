namespace Silo.Application.Dto;

public class InventoryReportRequest
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
    public string ProductCode { get; set; } = "-1";
    public string TechnicalCode { get; set; } = "-1";
    public string ProductType { get; set; } = "-1";
    public string ProductGroup { get; set; } = "-1";
    public string ProductSize { get; set; } = "-1";
    public string ProductStatus { get; set; } = "-1";
}
