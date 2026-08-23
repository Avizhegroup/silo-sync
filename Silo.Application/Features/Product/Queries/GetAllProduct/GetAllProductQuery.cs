namespace Silo.Application.Features;

public class GetAllProductQuery
{
    public string MProductTitle { get; set; }
    public string MProductCode { get; set; }
    public string MTechCode { get; set; }
    public string MSize { get; set; }
    public string MQuality { get; set; }
    public string Brand { get; set; } = "-1";
    public string Group { get; set; } = "-1";
    public string SubGroup { get; set; } = "-1";
    public string Class { get; set; } = "-1";
    public bool IsActive { get; set; } = true;
}
