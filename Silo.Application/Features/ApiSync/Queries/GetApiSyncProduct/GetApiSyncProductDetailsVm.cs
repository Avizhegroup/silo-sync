using System.Text.Json.Serialization;

namespace Silo.Application.Features;

public class GetApiSyncProductDetailsVm
{
    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_ProductSerial))]
    public string ProductSerial { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_ProductTitle))]
    public string ProductTitle { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_ProductRegDateTime))]
    public string RegisterDateTime { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_Tagzone))]
    public string TagZone { get; set; }
}

[JsonSerializable(typeof(ApiResponse<List<GetApiSyncProductDetailsVm>>))]
public partial class GetApiSyncProductDetailsContext : JsonSerializerContext
{

}
