using System.Text.Json.Serialization;

namespace Silo.Application.Features;
public class GetProductGuaranteeBySerialVm
{
    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_ExpireAndGuarantee_GuaranteeStatus))]
    public string GuaranteeStatusTitle { get; set; }

    public int GuaranteeStatus { get; set; }
    public  string Username { get; set; }

    public string GuaranteeStatusClass 
    {
        get => GuaranteeStatus switch
        {
            1 => " bg-success",
            2 => " bg-danger",
            _ => ""
        };
    }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_Guarantee_Start_Type))]
    public string GuaranteeActivationType { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_StartDate))]
    public string GuaranteeStartDate { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_EndDate))]
    public string GuaranteeEndDate { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_RemainingDay))]
    public string GuaranteeRemainingDay { get; set; }

    public double? RemainsDays
    {
        get
        {
            if (GuaranteeEndDate.HasValue())
            {
                var remainDays = (PersianCalendarTools.PersianToGregorian(GuaranteeEndDate) - DateTime.Now).TotalDays;

                return remainDays > 0 ? Math.Round(remainDays) : 0;
            }

            return null;
        }
    }
}
[JsonSerializable(typeof(ApiResponse<List<GetProductGuaranteeBySerialVm>>))]
public partial class GetProductGuaranteeBySerialVmContext : JsonSerializerContext
{

}
