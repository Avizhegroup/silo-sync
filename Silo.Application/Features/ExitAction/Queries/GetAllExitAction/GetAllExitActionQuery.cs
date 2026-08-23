namespace Silo.Application.Features;

public class GetAllExitActionQuery
{
    public string ProductCode { get; set; }
    public string OperationCode { get; set; }
    public string TechnicalCode { get; set; }
    public bool TechnicalCodeLike { get; set; } = false;
    public string FromDate { get; set; }
    public string ToDate { get; set; }
    public string RecordsCount { get; set; } = "100";
    public string GateOpCode { get; set; }
    public string StoreCode { get; set; }
    public int? ActionType { get; set; }
    public string Size { get; set; } = "-1";
    public string ProductGroup { get; set; }
    public string ProductBrand { get; set; }
    public string ProductType { get; set; }
    public string GateCode { get; set; }
    public string DocCode { get; set; }
}
