namespace Silo.Application.Features;

public class GetAllRevokeBySerialQuery
{
    public string ProductSerial { get; set; }
    public string FromDate { get; set; }
    public string ToDate { get; set; }
    public string User { get; set; }
}
