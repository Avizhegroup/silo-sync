namespace Silo.Application.Dto;

public class PlaqueParts
{
    [Display(ResourceType = typeof(TextResources), Name = "APP_StringKeys_Plaque")]
    [RegularExpression("[0-9][0-9]", ErrorMessageResourceType = typeof(TextResources), ErrorMessageResourceName = "APP_StringKeys_Validation_BadFormat")]
    public string FirstPart { get; set; }

    public string Character { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = "APP_StringKeys_Plaque")]
    [RegularExpression("[0-9][0-9][0-9]", ErrorMessageResourceType = typeof(TextResources), ErrorMessageResourceName = "APP_StringKeys_Validation_BadFormat")]
    public string SecondPart { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = "APP_StringKeys_Plaque")]
    [RegularExpression("[0-9][0-9]", ErrorMessageResourceType = typeof(TextResources), ErrorMessageResourceName = "APP_StringKeys_Validation_BadFormat")]
    public string CityPart { get; set; }
}

