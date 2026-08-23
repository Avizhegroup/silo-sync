namespace Silo.Application.Features;

public class GetProductDetailsByZoneAndProductCodeVm
{
    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_Count))]
    public int Count { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_SumValue))]
    public decimal SumCount { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_EnterDate))]
    public string EnterDate { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_Field_Warehouse_Code))]
    public string WarehouseCode { get; set; }
}
