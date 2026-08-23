using System.Text.Json.Serialization;

namespace Silo.Application.Features;

public class GetAllDocumentStatusVm
{
    public int Id { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_Field_Title))]
    public string Title { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_DocumentStatus_IsUpdatePermitted))]
    public bool IsUpdatePermitted { get; set; }

    [Display(ResourceType = typeof(TextResources) , Name = nameof(TextResources.APP_StringKeys_DocumentStatus_IsCartablePermitted))]
    public bool IsCartablePermitted { get; set; }

    [JsonIgnore]
    public bool IsChoosen { get; set; } = false;
}

[JsonSerializable(typeof(ApiResponse<List<GetAllDocumentStatusVm>>))]
public partial class GetAllDocumentStatusVmContext : JsonSerializerContext
{
}
