namespace Silo.Application.Features;

public class UpdateUserProfileCommand
{
    public string Password { get; set; }
    public string NewPassword { get; set; }
    public string Image { get; set; }
}
