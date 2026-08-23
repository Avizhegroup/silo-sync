namespace Silo.Application.Features;
public class GetGpsLogDto
{
    public int LogId { get; set; }
    public string UserId { get; set; }
    public string UserName { get; set; }
    public string Lat { get; set; }
    public string Long { get; set; }
    public GPSLogUsageType UsageType { get; set; }
    public DateTime LogDateTime { get; set; }
    public string UsageId { get; set; }
    public string AdditionalData { get; set; }
}
