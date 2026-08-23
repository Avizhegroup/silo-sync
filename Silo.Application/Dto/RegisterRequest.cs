namespace Silo.Application.Dto;

public class GetRegisterRequestFilter
{
    public string ProductCode { get; set; }
    public string Line { get; set; }
    public string FromDate { get; set; }
    public string ToDate { get; set; }
    public string User { get; set; }
    public string Shift { get; set; }
    public string Qc { get; set; }
    public string TechnicalCode { get; set; }
    public string ProductSerial { get; set; }
    public bool TechnicalCodeLike { get; set; } = false;
    public string Size { get; set; } = "-1";
    public string ContractStatus { get; set; }
    public string ProductGroup { get; set; }
    public string ProductBrand { get; set; }
    public string RegisterDevice { get; set; } = "-1";
    public string InspectStatus { get; set; }
    public string FromTime { get; set; }
    public string ToTime { get; set; }
    public string ProductOldSerial { get; set; }

}
