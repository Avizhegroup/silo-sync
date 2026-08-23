namespace Silo.Application.Features;

public class GetAllFreezeReportQuery
{
    public string FromDate { get; set; }
    public string ToDate { get; set; }
    public string UserId { get; set; }
    public string TechnicalCode { get; set; }
    public bool TechnicalCodeLike { get; set; }
    public string ProductCode { get; set; }
    public string ProductSerial { get; set; }
}