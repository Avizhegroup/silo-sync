namespace Silo.Application.Features;
public class CreateReportFormatCommand
{
    public ReportFormatTypes Type { get; set; }
    public string Path { get; set; }

    [Display(Name = nameof(TextResources.APP_StringKeys_Name), ResourceType = typeof(TextResources))]
    [Required(ErrorMessageResourceName = nameof(TextResources.APP_StringKeys_Validation_Required), ErrorMessageResourceType = typeof(TextResources))]
    public string Name { get; set; }

    public List<ReportFormatDetail> Details { get; set; } = new();
}
