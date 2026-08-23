namespace Silo.Application.Features;
public class TruckCrossConfigWithCause
{
    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_Code))]
    public int Id { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_Field_Title) )]
    [StringLength(128, MinimumLength = 1, ErrorMessageResourceType = typeof(TextResources), ErrorMessageResourceName = nameof(TextResources.APP_StringKeys_Validation_Stringlength))]
    [Required(ErrorMessageResourceType = typeof(TextResources), ErrorMessageResourceName = nameof(TextResources.APP_StringKeys_Validation_Required))]
    public string Title { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_TruckCross_Present_Cause) )]
    public int TruckCrossCauseId { get; set; }

    public List<int>? TruckCrossCauseIds { get; set; } = new();

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_TruckCross_Present_Causes))]
    public string? TruckCrossCauseTitles { get; set; }
}
