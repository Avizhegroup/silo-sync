namespace Silo.Application.Features;

public class TruckConfigDto
{
    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_Truck_Number))]
    [Required(ErrorMessageResourceName = nameof(TextResources.APP_StringKeys_Validation_Required), ErrorMessageResourceType = typeof(TextResources))]
    [RegularExpression("([1-9][0-9]*)", ErrorMessageResourceType = typeof(TextResources), ErrorMessageResourceName = nameof(TextResources.APP_StringKeys_Validation_Regex))]
    public string TruckNumber { get; set; }
}
