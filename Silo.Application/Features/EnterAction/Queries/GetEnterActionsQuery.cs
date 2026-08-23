namespace Silo.Application.Features;

public class GetEnterActionsQuery
{
    public string ProductCode { get; set; }
    public string OperationCode { get; set; }
    public string TechnicalCode { get; set; }
    public bool TechnicalCodeLike { get; set; } = false;
    public string FromDate { get; set; }
    public string ToDate { get; set; }
    public string RecordsCount { get; set; } = "100";
    public string GateOpCode { get; set; }
    public string Destination { get; set; }
    public int? ActionType { get; set; }
    public string Size { get; set; } = "-1";
    public string ProductGroup { get; set; }
    public string ProductBrand { get; set; }
    public string ProductType { get; set; }
    public string GateCode { get; set; }
    public string Qc { get; set; }
    public string Zone { get; set; }
    public string DocumentKey { get; set; }
}
