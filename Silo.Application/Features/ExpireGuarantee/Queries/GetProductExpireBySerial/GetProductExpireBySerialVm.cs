using System.Text.Json.Serialization;

namespace Silo.Application.Features;
public class GetProductExpireBySerialVm
{
    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_ExpireAndGuarantee_ExpireStatus))]
    public string ExpireStatusTitle { get; set; }

    public string Username { get; set; }

    public string ExpireStatusClass
    {
        get => ExpireStatus switch
        {
            1 => " bg-success",
            2 => " bg-danger",
            _ => ""
        };
    }

    public int ExpireStatus { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_Expire_Start_Type))]
    public string ExpireActivationType { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_StartDate))]
    public string ExpireStartDate { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_EndDate))]
    public string ExpireEndDate { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_RemainingDay))]
    public string ExpireRemainingDay { get; set; }

    public double? RemainsDays
    {
        get
        {
            if (ExpireEndDate.HasValue())
            {
                var remainDays = (PersianCalendarTools.PersianToGregorian(ExpireEndDate) - DateTime.Now).TotalDays;

                return remainDays > 0 ? Math.Round(remainDays) : 0;
            }

            return null;
        }
    }
}
[JsonSerializable(typeof(ApiResponse<List<GetProductExpireBySerialVm>>))]
public partial class GetProductExpireBySerialVmContext : JsonSerializerContext
{

}
