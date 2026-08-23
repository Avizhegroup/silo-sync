using System.Text.Json.Serialization;

namespace Silo.Application.Features;
public class GetMenuLinkOfDynamicReportVm
{
    public string Title { get; set; }
    public List<string> UserIds { get; set; } = new();
    public int? CategoryId { get; set; }
}

[JsonSerializable(typeof(ApiResponse<GetMenuLinkOfDynamicReportVm>))]
public partial class GetMenuLinkOfDynamicReportVmContext : JsonSerializerContext
{

}
