namespace Silo.Application.Features;

public class GetApiSyncProductQuery : ICloneable
{
    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_Warehouse))]
    [Required(ErrorMessageResourceType = typeof(TextResources), ErrorMessageResourceName = nameof(TextResources.APP_StringKeys_Validation_Required))]
    public string WarehouseCode { get; set; }
    public string Date { get; set; } = PersianCalendarTools.GregorianToPersian(DateTime.Now);
    public string GateCode { get; set; }
    public string ProductCode { get; set; }
    public string RegCode { get; set; }
    public string ProductType { get; set; }
    public string ProductQc { get; set; }
    public string ProductSize { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_ActionType))]
    [Required(ErrorMessageResourceType = typeof(TextResources), ErrorMessageResourceName = nameof(TextResources.APP_StringKeys_Validation_Required))]
    public int? ActionType { get; set; }

    public object Clone()
    {
        return this.MemberwiseClone();
    }
}
