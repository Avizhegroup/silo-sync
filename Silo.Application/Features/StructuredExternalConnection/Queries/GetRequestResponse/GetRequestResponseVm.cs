using System.Text.Json.Serialization;

namespace Silo.Application.Features;

public class GetRequestResponseVm
{
    public required ApiResponseWithStatusCode<JsonElement> Result { get; set; }
}

[JsonSerializable(typeof(ApiResponse<ApiResponseWithStatusCode<JsonElement>>))]
public partial class GetRequestResponseVmContext : JsonSerializerContext
{
}
