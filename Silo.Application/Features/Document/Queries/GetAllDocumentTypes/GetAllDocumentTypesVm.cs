using System.Text.Json.Serialization;

namespace Silo.Application.Features;
public class GetAllDocumentTypesVm
{
    public int Id { get; set; }
    public string? Code { get; set; }
    public string? Title { get; set; }
}

[JsonSerializable(typeof(ApiResponse<List<GetAllDocumentTypesVm>>))]
public partial class GetAllDocumentTypesVmContext : JsonSerializerContext
{

}
