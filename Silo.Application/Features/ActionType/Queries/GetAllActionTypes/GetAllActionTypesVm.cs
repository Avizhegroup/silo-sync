using System.Text.Json.Serialization;

namespace Silo.Application.Features;
public class GetAllActionTypesVm
{
    public List<GetAllActionTypesDto> List { get; set; }
}

[JsonSerializable(typeof(ApiResponse<GetAllActionTypesVm>))]
public partial class GetAllActionTypesVmContext : JsonSerializerContext
{

}
