using System.Text.Json.Serialization;

namespace Silo.Application.Features;

public class AddBulkUserClaimsVm
{
    public bool Succeeded { get; set; }
}

[JsonSerializable(typeof(ApiResponse<AddBulkUserClaimsVm>))]
public partial class AddBulkUserClaimsVmContext : JsonSerializerContext
{
}
