namespace Silo.Application.Features;

public class GetLoginUserQuery
{
    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_Field_Username))]
    [Required(ErrorMessageResourceType = typeof(TextResources), ErrorMessageResourceName = nameof(TextResources.APP_StringKeys_Validation_Required))]
    public string Username { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_Password))]
    [Required(ErrorMessageResourceType = typeof(TextResources), ErrorMessageResourceName = nameof(TextResources.APP_StringKeys_Validation_Required))]
    public string Password { get; set; }
}
