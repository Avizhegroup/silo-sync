using System.Text.Json.Serialization;

namespace Silo.Application.Features;
public class GetGuaranteeProductsVm
{
    public bool IsSelected { get; set; } = false;

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_ProductCode))]
    public string ProductCode { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_ProductName))]
    public string ProductName { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_Product_Size))]
    public string Size { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_Chart_Regcode))]
    public string TechnicalCode { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_QC))]
    public string Qc { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_ProductGroup))]
    public string ProductGroup { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_ProductBrand))]
    public string ProductBrand { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_ProductType))]
    public string ProductType { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_Expire_Type))]
    public string ExpireStatus { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_Expire_Duration))]
    public string ExpireDuration { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_Guarantee_Type))]
    public string GuaranteeStatus { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_Guarantee_Duration))]
    public string GuaranteeDuration { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_Product_SubGroup))]
    public string ProductSubGroupTitle { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_ProductClass))]
    public string ProductClassTitle { get; set; }
}

[JsonSerializable(typeof(ApiResponse<List<GetGuaranteeProductsVm>>))]
public partial class GetGuaranteeProductsVmContext : JsonSerializerContext
{

}
