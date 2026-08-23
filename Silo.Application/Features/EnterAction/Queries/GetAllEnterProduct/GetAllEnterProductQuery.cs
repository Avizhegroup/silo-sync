namespace Silo.Application.Features;

public class GetAllEnterProductQuery
{
    public string ProductCode { get; set; }
    public string ProductName { get; set; }
    public string FromDate { get; set; }
    public string ToDate { get; set; }
    public string FromTime { get; set; } = "-1";
    public string ToTime { get; set; } = "-1";
    public string Shift { get; set; } = "-1";
    public string ProductSerial { get; set; }
    public string TargetProductCode { get; set; }
    public string TechnicalCode { get; set; }
    public bool TechnicalCodeLike { get; set; } = false;
    public string Qc { get; set; } = "-1";
    public string Size { get; set; } = "-1";
    public string DestinationCode { get; set; }
    public int? ActionType { get; set; }
    public string ProductGroup { get; set; }
    public string ProductBrand { get; set; }
    public string ProductType { get; set; }
    public string GateCode { get; set; }
}
