using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Silo.Application.Features;
public class GetProductGuaranteesVm
{
    public bool IsSelected { get; set; } = false;

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_ProductSerial))]
    public string ProductSerial { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_OldSerial))]
    public string OldSerial { get; set; }
    
    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_ProductCode))]
    public string ProductCode { get; set; }
    
    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_Chart_Regcode))]
    public string RegCode { get; set; }
    
    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_ProductTitle))]
    public string ProductTitle { get; set; }
    
    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_ProductType))]
    public string ProductType { get; set; }
    
    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_ProductGroup))]
    public string ProductGroup { get; set; }
    
    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_Product_SubGroup))]
    public string ProductSubGroup { get; set; }
    
    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_ProductClass))]
    public string ProductClass { get; set; }
    
    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_ProductBrand))]
    public string ProductBrand { get; set; }
    
    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_Chart_Qc))]
    public string ProductStatus { get; set; }
    
    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_Chart_ProductDate))]
    public string RegisterShamsiDate { get; set; }
    
    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_Inspect_Status))]
    public string InspectStatusTitle { get; set; }
    
    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_Inspect_Date))]
    public string InspectDate { get; set; }
    
    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_EnterDate))]
    public string EnterActionDate { get; set; }
    
    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_ExitDate))]
    public string ExitActionDate { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_Guarantee_Type))]
    public string ActivationType { get; set; }
    
    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_ExpireAndGuarantee_GuaranteeStatus))]
    public string GuaranteeStatus { get; set; }
    
    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_ExpireAndGuarantee_StartDate))]
    public string GuaranteeStartDate { get; set; }
    
    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_ExpireAndGuarantee_EndDate))]
    public string GuaranteeEndDate { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_RemainingDay))]
    public string GuaranteeRemainingDays
    {
        get
        {
            if (GuaranteeEndDate.HasValue())
            {
                TimeSpan remainingTime = PersianCalendarTools.PersianToGregorian(GuaranteeEndDate) - DateTime.Now;
                return Math.Round(remainingTime.TotalDays, 0) + TextResources.APP_StringKeys_Day;
            }
            else
            {
                return null;
            }
        }
    }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_ExpireAndGuarantee_ExpireDays))]
    public string GuaranteeExpireDays
    {
        get
        {
            if (GuaranteeStartDate.HasValue() && GuaranteeEndDate.HasValue())
            {
                TimeSpan expireTime = PersianCalendarTools.PersianToGregorian(GuaranteeEndDate)
                                        - PersianCalendarTools.PersianToGregorian(GuaranteeStartDate);

                return Math.Round(expireTime.TotalDays, 0) + TextResources.APP_StringKeys_Day;
            }
            else
            {
                return null;
            }
        }
    }

}
[JsonSerializable(typeof(ApiResponse<List<GetProductGuaranteesVm>>))]
public partial class GetProductGuaranteesVmContext : JsonSerializerContext
{

}
