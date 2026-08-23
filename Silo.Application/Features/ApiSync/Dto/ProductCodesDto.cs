namespace Silo.Application.Features;

public class ProductCodesDto
{
    public string OldProductCode { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_Update_ProductCode))]
    [Required(ErrorMessageResourceType = typeof(TextResources), ErrorMessageResourceName = nameof(TextResources.APP_StringKeys_Validation_Required))]
    public string NewProductCode { get; set; }
}
