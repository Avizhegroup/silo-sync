namespace Silo.Application.Features;
public class TruckCrossItemDto
{
    public int Id { get; set; }
    public int? Type { get; set; }
    public string? Title { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_ProductUnit))]
    [Required(ErrorMessageResourceType = typeof(TextResources), ErrorMessageResourceName = nameof(TextResources.APP_StringKeys_Validation_Required))]
    public string? ProductUnit { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_ProductCount))]
    [Required(ErrorMessageResourceType = typeof(TextResources), ErrorMessageResourceName = nameof(TextResources.APP_StringKeys_Validation_Required))]
    public decimal? ProductCount { get; set; }
    public string? ProductSerial { get; set; }
    public string? ProductCode { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_Field_ProductType))]
    [Required(ErrorMessageResourceType = typeof(TextResources), ErrorMessageResourceName = nameof(TextResources.APP_StringKeys_Validation_Required))]
    public int? TruckCrossProductTypeId { get; set; }
    public string TruckCrossProductTypeTitle { get; set; }
    public long? TruckCrossId { get; set; }
    public bool IsDeleteMessageShown { get; set; } = false;
}
