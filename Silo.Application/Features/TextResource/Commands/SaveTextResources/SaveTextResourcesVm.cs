using System.Text.Json.Serialization;

namespace Silo.Application.Features;

public class SaveTextResourcesVm
{
    public bool Result { get; set; }
}

[JsonSerializable(typeof(ApiResponse<SaveTextResourcesVm>))]
public partial class SaveTextResourcesVmContext : JsonSerializerContext
{
}
