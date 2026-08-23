namespace Silo.Application.Features;
public class SaveMenuLinkOfDynamicReportCommand
{
    public int FormatId { get; set; }
    public string Url { get; set; }

    [Required(ErrorMessageResourceType = typeof(TextResources), ErrorMessageResourceName = nameof(TextResources.APP_StringKeys_Validation_Required))]
    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_Field_Title))]
    public string Title { get; set; }

    public int? SelectedCategoryId { get; set; }

    public List<string> UserIds { get; set; } = new();
}
