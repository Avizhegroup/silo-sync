namespace Silo.Application.Features;

public class CreateSyncSourceCommand : IRequest<CreateSyncSourceVm>
{
    [Required(ErrorMessageResourceType = typeof(TextResources), ErrorMessageResourceName = nameof(TextResources.APP_StringKeys_Validation_Required))]
    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_Code))]
    public string SourceKey { get; set; } = null!;

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_Title))]
    public string? DisplayName { get; set; }

    public string? SourceType { get; set; }

    [Required(ErrorMessageResourceType = typeof(TextResources), ErrorMessageResourceName = nameof(TextResources.APP_StringKeys_Validation_Required))]
    public string Command { get; set; } = null!;

    [Required(ErrorMessageResourceType = typeof(TextResources), ErrorMessageResourceName = nameof(TextResources.APP_StringKeys_Validation_Required))]
    public string FieldKey { get; set; } = null!;

    [Required(ErrorMessageResourceType = typeof(TextResources), ErrorMessageResourceName = nameof(TextResources.APP_StringKeys_Validation_Required))]
    public string FieldCheck { get; set; } = null!;

    [Required(ErrorMessageResourceType = typeof(TextResources), ErrorMessageResourceName = nameof(TextResources.APP_StringKeys_Validation_Required))]
    public string FieldOrder { get; set; } = null!;

    public int IntervalSeconds { get; set; } = 60;

    public bool IsEnabled { get; set; } = true;

    public string? ConnectionString { get; set; }
}
