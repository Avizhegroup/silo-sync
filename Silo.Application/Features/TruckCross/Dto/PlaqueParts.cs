namespace Silo.Application.Features;
public class PlaqueParts
{
    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_Plaque) )]
    [MinLength(2, ErrorMessageResourceType = typeof(TextResources),ErrorMessageResourceName = nameof(TextResources.APP_StringKeys_Validation_BadFormat))]
    [RegularExpression("[0-9][0-9]", ErrorMessageResourceType = typeof(TextResources), ErrorMessageResourceName = nameof(TextResources.APP_StringKeys_Validation_BadFormat))]
    [Required(ErrorMessageResourceType = typeof(TextResources), ErrorMessageResourceName = nameof(TextResources.APP_StringKeys_Validation_Required))]
    public string FirstPart { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_Plaque))]
    [Required(ErrorMessageResourceType = typeof(TextResources), ErrorMessageResourceName = nameof(TextResources.APP_StringKeys_Validation_Required))]
    public string Character { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_Plaque))]
    [Required(ErrorMessageResourceType = typeof(TextResources), ErrorMessageResourceName = nameof(TextResources.APP_StringKeys_Validation_Required))]
    [MinLength(3, ErrorMessageResourceType = typeof(TextResources),ErrorMessageResourceName = nameof(TextResources.APP_StringKeys_Validation_BadFormat))]
    [RegularExpression("[0-9][0-9][0-9]", ErrorMessageResourceType = typeof(TextResources), ErrorMessageResourceName = nameof(TextResources.APP_StringKeys_Validation_BadFormat))]
    public string SecondPart { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_Plaque))]
    [Required(ErrorMessageResourceType = typeof(TextResources), ErrorMessageResourceName = nameof(TextResources.APP_StringKeys_Validation_Required))]
    [MinLength(2, ErrorMessageResourceType = typeof(TextResources),ErrorMessageResourceName = nameof(TextResources.APP_StringKeys_Validation_BadFormat))]
    [RegularExpression("[0-9][0-9]", ErrorMessageResourceType = typeof(TextResources), ErrorMessageResourceName = nameof(TextResources.APP_StringKeys_Validation_BadFormat))]
    public string CityPart { get; set; }
}
