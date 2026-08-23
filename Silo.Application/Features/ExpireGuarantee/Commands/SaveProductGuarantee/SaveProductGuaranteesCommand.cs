namespace Silo.Application.Features;
public class SaveProductGuaranteesCommand
{
    public List<string> ProductSerials { get; set; } = new();

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_ExpireAndGuarantee_GuaranteeStatus) )]
    [Required(ErrorMessageResourceType = typeof(TextResources), ErrorMessageResourceName = nameof(TextResources.APP_StringKeys_Validation_Required))]
    public string GuaranteeStatus { get; set; }
    
    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_ExpireAndGuarantee_StartDate))]
    [Required(ErrorMessageResourceType = typeof(TextResources), ErrorMessageResourceName = nameof(TextResources.APP_StringKeys_Validation_Required))]
    public string GuaranteeStartDate { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_ExpireAndGuarantee_EndDate))]
    [Required(ErrorMessageResourceType = typeof(TextResources), ErrorMessageResourceName = nameof(TextResources.APP_StringKeys_Validation_Required))]
    public string GuaranteedEndDate { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_Guarantee_Type))]
    [Range(0, 100, ErrorMessageResourceType = typeof(TextResources), ErrorMessageResourceName = nameof(TextResources.APP_StringKeys_Validation_Required))]
    public GuaranteeTypes GuaranteeActivationType { get; set; } = GuaranteeTypes.None;
}
