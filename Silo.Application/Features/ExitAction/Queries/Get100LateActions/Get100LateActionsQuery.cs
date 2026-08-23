using System.Text.Json.Serialization;

namespace Silo.Application.Features;

public class Get100LateActionsQuery
{
    public int Code { get; set; }
    public int Count { get; set; }
    public string DateTime { get; set; }
    public string Status { get; set; }
}

[JsonSerializable(typeof(ApiResponse<List<Get100LateActionsQuery>>))]
public partial class Get100LateActionsQueryContext : JsonSerializerContext
{
}
