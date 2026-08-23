namespace Silo.Application.Features;
public class GetLocateProductQuery
{
    public string ProductCode { get; set; }
    public string TechnicalCode { get; set; } = "-1";
    public bool TechnicalCodeLike { get; set; } = false;
    public string Size { get; set; } = "-1";
    public int RecordsCount { get; set; }
    public string Destination { get; set; }
    public string ProductGroup { get; set; } = "-1";
    public string ProductBrand { get; set; } = "-1";
    public string ProductType { get; set; } = "-1";
    public string Qc { get; set; } = "-1";
    public string Zone { get; set; } = "-1";
    public string FromDate { get; set; } = "-1";
    public string ToDate { get; set; } = "-1";
}
