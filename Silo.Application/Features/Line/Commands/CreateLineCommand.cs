namespace Silo.Application.Features;

public class CreateLineCommand 
{
    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_Code))]
    [StringLength(128)]
    [Required(ErrorMessageResourceType = typeof(TextResources), ErrorMessageResourceName = nameof(TextResources.APP_StringKeys_Validation_Required))]
    public string Code { get; set; }

    [Required(ErrorMessageResourceType = typeof(TextResources), ErrorMessageResourceName = nameof(TextResources.APP_StringKeys_Validation_Required))]
    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_Field_Title))]
    [StringLength(256)]
    public string Title { get; set; }
    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_Description))]
    [StringLength(512)]
    public string? Desc { get; set; }
}
