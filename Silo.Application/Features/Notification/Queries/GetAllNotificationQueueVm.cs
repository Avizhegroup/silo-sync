using System.Text.Json.Serialization;

namespace Silo.Application.Features;

public class GetAllNotificationQueueVm
{
    public DateTime? SendDateTime { get; set; }
    public string? SendDate { get; set; }
    public string? SendTime { get; set; }
    public string SendType { get; set; }
    public int OrderId { get; set; }
    public string OrderTitle { get; set; }
    public string Contact { get; set; }
    public string Text { get; set; }
    public string Status { get; set; }
}
[JsonSerializable(typeof(ApiResponse<List<GetAllNotificationQueueVm>>))]
public partial class GetAllNotificationQueueVmContext : JsonSerializerContext
{
}
