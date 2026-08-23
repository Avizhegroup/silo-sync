namespace Silo.Application.Features;
public class GetPlaceByActionIdVm
{
    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_OperationCode))]
    public int MovementActionId { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_Date))]
    public string MovementActionDate { get; set; }
    
    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_Time))]
    public string MovementActionTime { get; set; }
    
    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_ActionType))]
    public string ActionTypeTitle { get; set; }
    
    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_Destination_Warehouse))]
    public string DestinationTitle { get; set; }
    
    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_DocKey))]
    public string MovementActionDocumentId { get; set; }
    
    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_Field_Description))]
    public string MovementActionDesc { get; set; }
    
    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_ProductSerial))]
    public string ProductSerial { get; set; }
    
    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_ProductCode))]
    public string ProductCode { get; set; }
    
    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_ProductName))]
    public string ProductName { get; set; }
    
    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_ProductValue))]
    public decimal ProductCount { get; set; }
    
    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_Chart_Regcode))]
    public string RegCode { get; set; }
}
