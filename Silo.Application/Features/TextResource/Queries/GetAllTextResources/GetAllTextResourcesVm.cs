using System.Text.Json.Serialization;

namespace Silo.Application.Features;

public class GetAllTextResourcesVm
{
    public int Id { get; set; }

    public string Key { get; set; } = null!;

    public string? Value { get; set; }
}

[JsonSerializable(typeof(ApiResponse<List<GetAllTextResourcesVm>>))]
public partial class GetAllTextResourcesVmContext : JsonSerializerContext
{
}
