namespace Silo.Application.Features;

public class GetAllProductBySerialQuery
{
    public string FromSerial { get; set; }
    public string ToSerial { get; set; }
    public string Line { get; set; }
    public string ProductCode { get; set; }
    public string TechnicalCode { get; set; }
    public bool TechnicalCodeLike { get; set; }
    public string FromDate { get; set; }
    public string ToDate { get; set; }
    public string Shift { get; set; }
    public string Size { get; set; }
    public string Qc { get; set; }
    public int FreezeStatus { get; set; } = -1;
    public string OldSerial { get; set; }
    public string Type { get; set; }
    public string Group { get; set; }
    public string Brand { get; set; }
    public Dictionary<string, string> DynamicFilters { get; set; } = new();
}
