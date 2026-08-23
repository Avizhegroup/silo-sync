using System.Text.Json.Serialization;

namespace Silo.Application.Features;

public class GetTagRelatedTagsVm
{
    public string ProductSerial { get; set; }
    public string TagEpc { get; set; }
    public string ProductName { get; set; }
    public string ProductCode { get; set; }
    public decimal ProductCount { get; set; }
    public string TagStatusTitle { get; set; }
    public string RegisterDate { get; set; }
    public string RelationType { get; set; }
}

[JsonSerializable(typeof(ApiResponse<List<GetTagRelatedTagsVm>>))]
public partial class GetTagRelatedTagsVmContext : JsonSerializerContext
{
}
