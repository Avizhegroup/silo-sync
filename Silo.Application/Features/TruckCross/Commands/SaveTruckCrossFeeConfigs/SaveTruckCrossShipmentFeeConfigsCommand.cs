namespace Silo.Application.Features;

public class SaveTruckCrossShipmentFeeConfigsCommand
{
    public int Id { get; set; }

    [Display(Name = nameof(TextResources.APP_StringKeys_Company), ResourceType = typeof(TextResources))]
    [Required(ErrorMessageResourceType = typeof(TextResources), ErrorMessageResourceName = nameof(TextResources.APP_StringKeys_Validation_Required))]
    public int? CompanyId { get; set; }

    [Display(Name = nameof(TextResources.APP_StringKeys_TruckCross_Customer), ResourceType = typeof(TextResources))]
    public int? CustomerId { get; set; }

    [Display(Name = nameof(TextResources.APP_StringKeys_Field_ProductType), ResourceType = typeof(TextResources))]
    public int? ProductTypeId { get; set; }

    [Display(Name = nameof(TextResources.APP_StringKeys_TruckCross_Shipment), ResourceType = typeof(TextResources))]
    public int? ShipmentId { get; set; }

    [Display(Name = nameof(TextResources.APP_StringKeys_FromDate), ResourceType = typeof(TextResources))]
    public string? FromDate { get; set; }

    [Display(Name = nameof(TextResources.APP_StringKeys_ToDate), ResourceType = typeof(TextResources))]
    public string? ToDate { get; set; }

    [Display(Name = nameof(TextResources.APP_StringKeys_ActivationStatus), ResourceType = typeof(TextResources))]
    public bool FeeStatus { get; set; }

    [Display(Name = nameof(TextResources.APP_StringKeys_Value), ResourceType = typeof(TextResources))]
    [Range(1, 1000000000000, ErrorMessageResourceType = typeof(TextResources), ErrorMessageResourceName = nameof(TextResources.APP_StringKeys_Validation_Required))]
    public decimal FeeAmount { get; set; }

    [Display(Name = nameof(TextResources.APP_StringKeys_Weight), ResourceType = typeof(TextResources))]
    public decimal FeeWeight { get; set; }

    [Display(Name = nameof(TextResources.APP_StringKeys_TruckCross_Exit_Distance), ResourceType = typeof(TextResources))]
    public decimal FeeDistance { get; set; }
}
