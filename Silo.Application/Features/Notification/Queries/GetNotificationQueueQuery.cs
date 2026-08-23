namespace Silo.Application.Features;

public class GetNotificationQueueQuery
{
    public string FromDate { get; set; }
    public string ToDate { get; set; }
    public string FromTime { get; set; }
    public string ToTime { get; set; }
    public string SendType { get; set; }
    public string OrderId { get; set; }
    public string OrderTitle { get; set; }
    public string SendContacts { get; set; }
    public string Content { get; set; }
    public string SendStatus { get; set; }
}
