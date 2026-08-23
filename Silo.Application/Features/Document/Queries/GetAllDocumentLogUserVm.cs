using System.Text.Json.Serialization;

namespace Silo.Application.Features;
public class GetAllDocumentLogUserVm
{
    public string? Code { get; set; }
    public string? Title { get; set; }
}

[JsonSerializable(typeof(ApiResponse<List<GetAllDocumentLogUserVm>>))]
public partial class GetAllDocumentLogUserVmContext : JsonSerializerContext
{
}
