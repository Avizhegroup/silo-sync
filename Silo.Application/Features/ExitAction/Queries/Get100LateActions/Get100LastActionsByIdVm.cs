using System.Text.Json.Serialization;

namespace Silo.Application.Features;

public class Get100LastActionsByIdVm
{
    public int Code { get; set; }
    public int Count { get; set; }
    public string DateTime { get; set; }
    public string Status { get; set; }
    public string GateCode { get; set; }
    public string StationName { get; set; }
}

[JsonSerializable(typeof(ApiResponse<List<Get100LastActionsByIdVm>>))]
public partial class Get100LastActionsByIdVmContext : JsonSerializerContext
{
}
