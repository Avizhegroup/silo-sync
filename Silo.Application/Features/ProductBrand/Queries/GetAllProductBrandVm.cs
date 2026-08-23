namespace Silo.Application.Features;

public class GetAllProductBrandVm
{
    public int Id { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_Field_Code))]
    [StringLength(128, MinimumLength = 1, ErrorMessageResourceType = typeof(TextResources), ErrorMessageResourceName = nameof(TextResources.APP_StringKeys_Validation_Stringlength))]
    [Required(ErrorMessageResourceType = typeof(TextResources), ErrorMessageResourceName = nameof(TextResources.APP_StringKeys_Validation_Required))]
    public string Code { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_Field_Title))]
    [StringLength(128,MinimumLength =1, ErrorMessageResourceType = typeof(TextResources), ErrorMessageResourceName = nameof(TextResources.APP_StringKeys_Validation_Stringlength))]
    [Required(ErrorMessageResourceType = typeof(TextResources), ErrorMessageResourceName = nameof(TextResources.APP_StringKeys_Validation_Required))]
    public string Title { get; set; }

    public string? Data { get; set; }
}
