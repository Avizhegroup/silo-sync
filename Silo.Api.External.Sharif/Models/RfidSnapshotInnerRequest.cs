namespace Silo.Api.External.Sharif.Models;

public class RfidSnapShotOutterRequest
{
    public RfidSnapshotInnerRequest Data { get; set; }
}
public class RfidSnapshotInnerRequest
{
    public string KioskId { get; set; } = string.Empty;
    public string ReaderId { get; set; } = string.Empty;
    public long SequenceNo { get; set; }
    public DateTime CapturedAt { get; set; }
    public List<RfidSnapshotTag> Tags { get; set; } = new();
}

public class RfidSnapshotTag
{
    public string uid { get; set; } = string.Empty;
}
