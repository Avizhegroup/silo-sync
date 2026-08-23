using System.Text.Json.Serialization;

namespace Silo.Application.Features;
public class GetTablesChangeLogVm
{
    public List<GetTablesChangeLogDto> List { get; set; }
}

[JsonSerializable(typeof(ApiResponse<GetTablesChangeLogVm>))]
public partial class GetTablesChangeLogVmContext : JsonSerializerContext
{
}
