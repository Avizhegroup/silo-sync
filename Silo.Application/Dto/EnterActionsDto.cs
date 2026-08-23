namespace Silo.Application.Dto;

public class EnterActionRequest
{
    public string OperationCode { get; set; }
    public string ProductCode { get; set; }
    public string FromDate { get; set; }
    public string ToDate { get; set; }
    public string TechnicalCode { get; set; } = "-1";
    public bool TechnicalCodeLike { get; set; } = false;
    public string Size { get; set; } = "-1";
    public int RecordsCount { get; set; }
    public string GateOpCode { get; set; }
    public string Destination { get; set; }
    public string ProductGroup { get; set; } = "-1";
    public string ProductBrand { get; set; } = "-1";
    public string ProductType { get; set; } = "-1";
    public string GateCode { get; set; } = "-1";
    public string Qc { get; set; } = "-1";
    public int? ActionType { get; set; }
    public string DocumentKey { get; set; } = "-1";
}

public class EnterActionAggOnProductCodeRequest
{
    public string ProductCode { get; set; }
    public string TechnicalCode { get; set; }
    public string FromDate { get; set; }
    public string ToDate { get; set; }
    public string FromTime { get; set; }
    public string ToTime { get; set; }
    public int RecordsCount { get; set; }
    public string DocCode { get; set; }
    public string GateOpCode { get; set; }
    public string Plaque { get; set; }
    public string Driver { get; set; }
    public string ProductSerial { get; set; }
    public string ProductName { get; set; }
}

