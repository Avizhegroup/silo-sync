using System.Text.Json.Serialization;

namespace Silo.Application.Features;

public class GetAllEnterProductVm
{
    [Display(Name = nameof(TextResources.APP_StringKeys_ProductCode), ResourceType = typeof(TextResources))]
    public string ProductCode { get; set; }

    [Display(Name = nameof(TextResources.APP_StringKeys_ProductName), ResourceType = typeof(TextResources))]
    public string ProductName { get; set; }

    [Display(Name = nameof(TextResources.APP_StringKeys_Count), ResourceType = typeof(TextResources))]
    public int Count { get; set; }

    [Display(Name = nameof(TextResources.APP_StringKeys_SumValue), ResourceType = typeof(TextResources))]
    public decimal SumCount { get; set; }

    [Display(Name = nameof(TextResources.APP_StringKeys_Chart_Regcode), ResourceType = typeof(TextResources))]
    public string RegCode { get; set; }

    [Display(Name = nameof(TextResources.APP_StringKeys_QC), ResourceType = typeof(TextResources))]
    public string Qc { get; set; }

    [Display(Name = nameof(TextResources.APP_StringKeys_Product_Size), ResourceType = typeof(TextResources))]
    public string Size { get; set; }

    [Display(Name = nameof(TextResources.APP_StringKeys_ProductGroup), ResourceType = typeof(TextResources))]
    public string ProductGroup { get; set; }

    [Display(Name = nameof(TextResources.APP_StringKeys_ProductBrand), ResourceType = typeof(TextResources))]
    public string ProductBrand { get; set; }

    [Display(Name = nameof(TextResources.APP_StringKeys_Type), ResourceType = typeof(TextResources))]
    public string ProductType { get; set; }

    [Display(Name = nameof(TextResources.APP_StringKeys_Station), ResourceType = typeof(TextResources))]
    public string GateCode { get; set; }

    public int? ActionType { get; set; }

    [Display(Name = nameof(TextResources.APP_StringKeys_ActionType), ResourceType = typeof(TextResources))]
    public string ActionTypeTitle { get; set; }

    public string DestinationCode { get; set; }

    public string DestinationTitle { get; set; }

    [Display(Name = nameof(TextResources.APP_StringKeys_Field_Warehouse_Title), ResourceType = typeof(TextResources))]
    public string StoreTitle { get; set; }
}

[JsonSerializable(typeof(ApiResponse<List<GetAllEnterProductVm>>))]
public partial class GetAllEnterProductVmContext : JsonSerializerContext
{
}
