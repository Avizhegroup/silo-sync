using System.Text.Json.Serialization;

namespace Silo.Application.Features;

public class Get100LastActionsVm
{
    public int Code { get; set; }
    public int Count { get; set; }
    public string DateTime { get; set; }
    public string Status { get; set; }
}

[JsonSerializable(typeof(ApiResponse<List<Get100LastActionsVm>>))]
public partial class Get100LastActionsVmContext : JsonSerializerContext
{
}
