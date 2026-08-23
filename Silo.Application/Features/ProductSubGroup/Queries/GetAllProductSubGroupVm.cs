using System.Text.Json.Serialization;

namespace Silo.Application.Features;

public class GetAllProductSubGroupVm
{
    public int Id { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_Field_Code))]
    [StringLength(128, MinimumLength = 1, ErrorMessageResourceType = typeof(TextResources), ErrorMessageResourceName = nameof(TextResources.APP_StringKeys_Validation_Stringlength))]
    [Required(ErrorMessageResourceType = typeof(TextResources), ErrorMessageResourceName = nameof(TextResources.APP_StringKeys_Validation_Required))]
    public string Code { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_Field_Title))]
    [StringLength(256, MinimumLength = 1, ErrorMessageResourceType = typeof(TextResources), ErrorMessageResourceName = nameof(TextResources.APP_StringKeys_Validation_Stringlength))]
    [Required(ErrorMessageResourceType = typeof(TextResources), ErrorMessageResourceName = nameof(TextResources.APP_StringKeys_Validation_Required))]
    public string Title { get; set; }

    [StringLength(512)]
    public string? SubTitle { get; set; }

    [StringLength(512)]
    public string? Description { get; set; }

    [StringLength(128)]
    public string ProductGroupCode { get; set; }

    public string ProductGroupTitle { get; set; }
}
[JsonSerializable(typeof(ApiResponse<List<GetAllProductSubGroupVm>>))]
[JsonSerializable(typeof(Dictionary<string, object>))]
[JsonSerializable(typeof(object))]
[JsonSerializable(typeof(string))]
public partial class GetAllProductSubGroupVmContext : JsonSerializerContext
{
}
