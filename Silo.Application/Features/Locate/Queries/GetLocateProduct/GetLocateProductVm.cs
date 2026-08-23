using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Silo.Application.Features;
public class GetLocateProductVm
{
    public bool IsSelected { get; set; } = false;
    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_ProductSerial))]
    public string ProductSerial { get; set; }

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

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_ProductValue))]
    public decimal Count { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_Line))]
    public string Line { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_Chart_Shift))]
    public string Shift { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_ProductGroup))]
    public string ProductGroup { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_ProductBrand))]
    public string ProductBrand { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_ProductType))]
    public string ProductType { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_Warehouse))]
    public string StoreTitle { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_Zone))]
    public string ZoneTitle { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_Product_Add_TechnicalInfo))]
    public string TechnicalInfoData { get; set; }
}

[JsonSerializable(typeof(ApiResponse<List<GetLocateProductVm>>))]
public partial class GetLocateProductVmContext  : JsonSerializerContext
{
    
}
