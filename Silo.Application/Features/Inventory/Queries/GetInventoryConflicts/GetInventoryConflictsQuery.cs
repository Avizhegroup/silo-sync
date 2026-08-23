namespace Silo.Application.Features;
public class GetInventoryConflictsQuery
{
    public string ProductCode { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_FromDate))]
    [Required(ErrorMessageResourceType = typeof(TextResources), ErrorMessageResourceName = nameof(TextResources.APP_StringKeys_Validation_Required))]
    public string FromDate { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_ToDate))]
    [Required(ErrorMessageResourceType = typeof(TextResources), ErrorMessageResourceName = nameof(TextResources.APP_StringKeys_Validation_Required))]
    public string ToDate { get; set; }
    public string TechnicalCode { get; set; }
    public string Qc { get; set; }
    public string User { get; set; }
    public string Desc { get; set; }
    public string Place { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_OperationCode))]
    [Required(ErrorMessageResourceType = typeof(TextResources), ErrorMessageResourceName = nameof(TextResources.APP_StringKeys_Validation_Required))]
    public string InventoryHeaderId { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_Warehouse))]
    [Required(ErrorMessageResourceType = typeof(TextResources), ErrorMessageResourceName = nameof(TextResources.APP_StringKeys_Validation_Required))]
    public string Warehouse { get; set; }
    public bool ConflictsShown { get; set; } = false;
    public string Size { get; set; }
    public string Type { get; set; }
    public bool TechnicalCodeLike { get; set; } = false;
    public bool IsMovementFiltered { get; set; } = false;
}
