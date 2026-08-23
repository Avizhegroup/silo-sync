
namespace Silo.Application.Shared.Dto;
public class GPSLogs
{
    public int LogId { get; set; }
    public string UserId { get; set; }
    public string Lat { get; set; }
    public string Long { get; set; }
    public int UsageType { get; set; }
    public DateTime LogDateTime { get; set; }
    public string UsageId { get; set; }
    public string AdditionalData { get; set; }

}

