using System.Text.Json.Serialization;

namespace Silo.Application.Features;

public class GetAllLinesVm
{

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_Code))]
    [Required(ErrorMessageResourceType = typeof(TextResources), ErrorMessageResourceName = nameof(TextResources.APP_StringKeys_Validation_Required))]
    public string Code { get; set; }
    [Required(ErrorMessageResourceType = typeof(TextResources), ErrorMessageResourceName = nameof(TextResources.APP_StringKeys_Validation_Required))]

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_Field_Title))]
    public string Title { get; set; }
    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_Description))]
    public string? Desc { get; set; }
    public string? Data { get; set; }
}

[JsonSerializable(typeof(ApiResponse<List<GetAllLinesVm>>))]
public partial class GetLineContext :JsonSerializerContext
{
}
