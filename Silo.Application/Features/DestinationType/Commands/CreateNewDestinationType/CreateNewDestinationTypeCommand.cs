namespace Silo.Application.Shared.Features;
public class CreateNewDestinationTypeCommand:IRequest<CreateNewDestinationTypeVm>
{
    public int? Id { get; set; }

    [Required(ErrorMessageResourceType = typeof(TextResources), ErrorMessageResourceName = nameof(TextResources.APP_StringKeys_Validation_Required))]
    public string? Code { get; set; }

    [Required(ErrorMessageResourceType = typeof(TextResources), ErrorMessageResourceName = nameof(TextResources.APP_StringKeys_Validation_Required))]
    public string? Title { get; set; }

}
