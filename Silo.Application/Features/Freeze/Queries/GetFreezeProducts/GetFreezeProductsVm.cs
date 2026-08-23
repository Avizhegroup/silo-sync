using System.Text.Json.Serialization;

namespace Silo.Application.Features;
public class GetFreezeProductsVm
{
    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_ProductSerial))]
    public string ProductSerial { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_ProductCode))]
    public string ProductCode { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_ProductName))]
    public string ProductName { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_ProductCount))]
    public decimal ProductCount { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_Chart_Regcode))]
    public string Regcode { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_QC))]
    public string Qc { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_Product_Size))]
    public string Size { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_Line))]
    public string Line { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_Freeze_User))]
    public string User { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_DateTime))]
    public DateTime? DateTime { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_Description))]
    public string Desc { get; set; }
}

[JsonSerializable(typeof(ApiResponse<List<GetFreezeProductsVm>>))]
public partial class GetFreezeProductsVmContext : JsonSerializerContext
{
}
