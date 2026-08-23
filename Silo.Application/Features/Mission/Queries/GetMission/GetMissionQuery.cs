namespace Silo.Application.Features;

public class GetMissionQuery
{
    public string ProductCode { get; set; }
    public string ProductSerial { get; set; }
    public string TechnicalCode { get; set; }
    public string FromDate { get; set; }
    public string ToDate { get; set; }
    public string FromZone { get; set; }
    public string ToZone { get; set; }
    public string Driver { get; set; }
    public string MissionCode { get; set; }
    public string MissionStatus { get; set; }
    public string MissionType { get; set; }
    public string ProductStatus { get; set; }
    public bool TechnicalCodeLike { get; set; } = false;
}
