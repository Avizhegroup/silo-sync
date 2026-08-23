using System.Text.Json.Serialization;

namespace Silo.Application.Features;

public class GetApiSyncProductVm
{
    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_ProductTypeCode))]
    public string ProductTypeCode { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_ProductTypeTitle))]
    public string ProductTypeTitle { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_ProductCode))]
    public string ProductCode { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_ProductTitle))]
    public string ProductTitle { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_Chart_Regcode))]
    public string RegCode { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_Chart_Qc))]
    public string Qc { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_Product_Size))]
    public string Size { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_Count))]
    public int Count { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_SeconValueInPack))]
    public decimal ProductCountInPack { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_SumValue))]
    public decimal SumCount { get; set; }

    public decimal ProductValue { get; set; }
    public int AvgApiSendStatus { get; set; }
}
//ردیف - کد سالن تولید – عنوان سالن تولید – کد کالا – کد فنی – عنوان کالا –
//درجه – سایز – تعداد – تعداد واحد دوم – مقدار کل 

[JsonSerializable(typeof(ApiResponse<List<GetApiSyncProductVm>>))]
public partial class GetApiSyncProductVmContext : JsonSerializerContext
{

}
