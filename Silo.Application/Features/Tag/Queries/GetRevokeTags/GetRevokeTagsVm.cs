using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Silo.Application.Features;
public class GetRevokeTagsVm
{
    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_ProductSerial))]
    public string ProductSerial { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_ProductCode))]
    public string ProductCode { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_Chart_Regcode))]
    public string RegCode { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_ProductName))]
    public string ProductName { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_Chart_Qc))]
    public string Qc { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_Product_Size))]
    public string Size { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_Field_ProductType))]
    public string ProductType { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_ProductGroup))]
    public string ProductGroup { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_ProductBrand))]
    public string ProductBrand { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_Value))]
    public decimal Count { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_Chart_ProductDate))]
    public string ProductDate { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_User))]
    public string User { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_Inspect_Status))]
    public string InspectStatus { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_Freeze_Status))]
    public string FreezeStatus { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_Field_Warehouse_Title))]
    public string Warehouse { get; set; }
    public string TagEpc { get; set; }
}

[JsonSerializable(typeof(ApiResponse<List<GetRevokeTagsVm>>))]
public partial class GetRevokeTagsVmContext : JsonSerializerContext
{
}
