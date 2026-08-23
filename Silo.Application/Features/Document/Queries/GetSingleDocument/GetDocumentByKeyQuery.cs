namespace Silo.Application.Features;
public class GetDocumentByKeyQuery
{
    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_Header_Key))]
    [Required(ErrorMessageResourceType = typeof(TextResources), ErrorMessageResourceName = nameof(TextResources.APP_StringKeys_Validation_Required))]
    public string DocumentKey { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_Document_Type))]
    [Required(ErrorMessageResourceType = typeof(TextResources), ErrorMessageResourceName = nameof(TextResources.APP_StringKeys_Validation_Required))]
    public int? DocumentType { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_Document_Type))]
    public int? DocumentCheckType { get; set; }
}
