namespace Silo.Application.Features;
public class CheckCustomerGuaranteeVm
{
    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_ProductTitle))]
    public string ProductTitle { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_ProductSerial))]
    public string ProductSerial { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_ExpireAndGuarantee_GuaranteeStatus))]
    public int GuaranteeStatus { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_ExpireAndGuarantee_GuaranteeStatus))]
    public string GuaranteeStatusTitle { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_Guarantee_Start_Type))]
    public string GuaranteeActivationTypeString { get; set; }

    public GuaranteeTypes GuaranteeActivationType { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_StartDate))]
    public string GuaranteeStartDate { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_EndDate))]
    public string GuaranteeEndDate { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_RemainingDay))]
    public double? GuaranteeRemainingDays
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

    public CustomerCheckGuaranteePageMode GuaranteeCheckResultStatus { get; set; }
}
