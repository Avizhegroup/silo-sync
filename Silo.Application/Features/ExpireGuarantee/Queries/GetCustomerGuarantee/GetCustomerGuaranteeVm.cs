namespace Silo.Application.Features;
public class GetCustomerGuaranteeVm
{
    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_ProductTitle))]
    public string ProductTitle { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_StartDate))]
    public string GuaranteeStartDate { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_EndDate))]
    public DateTime GuaranteeCheckDateTime { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_Guarantee_Type))]
    public GuaranteeTypes ActivationType { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_Product_Serial))]
    public string ProductSerial { get; set; }
}
