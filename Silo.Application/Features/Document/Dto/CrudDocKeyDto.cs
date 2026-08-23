namespace Silo.Application.Features;

public class CrudDocKeyDto
{
    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_DocKey))]
    [Required(ErrorMessageResourceType = typeof(TextResources), ErrorMessageResourceName = nameof(TextResources.APP_StringKeys_Validation_Required))]
    public string AddDocKey { get; set; }
    public string AddDocType { get; set; }
    public string RemoveDocKey { get; set; }
    public string RemoveDocType { get; set; }
}
