namespace Silo.Application.Features;
public class CreateNewActionTypeCommand : IRequest<CreateNewActionTypeVm>
{
    public int? Id { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_Code))]
    public int? Code { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_Title))]
    [Required(ErrorMessageResourceType = typeof(TextResources), ErrorMessageResourceName = nameof(TextResources.APP_StringKeys_Validation_Required))]
    public string? Title { get; set; }

    public int? RfidPower { get; set; }
    public List<string> ChoosenFromWarehouseTypes { get; set; } = new();
    public List<string> ChoosenToWarehouseTypes { get; set; } = new();
    public List<int> ChoosenDocumentChangeStatuses { get; set; } = new();
    public List<int> ChoosenDocumentPermittedStatuses { get; set; } = new();
    public List<string> ChoosenActionControls { get; set; } = new();
    public string? DocStatusPermitted { get; set; }
    public string? DocStatusChange { get; set; }
    public string? ActiveControls { get; set; }
    public string? To { get; set; }
    public string? From { get; set; }
}
