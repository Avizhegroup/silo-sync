using System.Text.Json.Serialization;

namespace Silo.Application.Features;
public class GetDocumentByKeyVm
{
    public string Key { get; set; }
    public string DocumentType { get; set; }
    public string DocumentType1 { get; set; }
    public string DocumentType2 { get; set; }
    public string HeaderData { get; set; }
    public int AggStatus { get; set; }
    public string Parent { get; set; }
    public int? DocumentCheckType { get; set; }
    public List<GetSingleDocumentItemsVm> DocumentItems { get; set; }
}

[JsonSerializable(typeof(ApiResponse<GetDocumentByKeyVm>))]
public partial class GetDocumentByKeyVmContext : JsonSerializerContext
{
}
