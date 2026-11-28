namespace Silo.Application.Features;

public class GetRequestResponseQuery : IRequest<GetRequestResponseVm>
{
    [Required(ErrorMessageResourceType = typeof(TextResources), ErrorMessageResourceName = nameof(TextResources.APP_StringKeys_Validation_Required))]
    public string CommandUniqueId { get; set; } = string.Empty;
}
