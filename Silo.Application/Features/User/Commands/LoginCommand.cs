namespace Silo.Application.Features;

public class LoginCommand
{
    [Display(ResourceType = typeof(TextResources), Name = "APP_StringKeys_Username")]
    [Required(ErrorMessageResourceType = typeof(TextResources), ErrorMessageResourceName = "APP_StringKeys_Validation_Required")]
    public string UserName { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = "APP_StringKeys_Password")]
    [Required(ErrorMessageResourceType = typeof(TextResources), ErrorMessageResourceName = "APP_StringKeys_Validation_Required")]
    public string Password { get; set; }
}
