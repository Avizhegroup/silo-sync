using Microsoft.AspNetCore.Identity;

namespace Silo.Identity.Client;

public class ApplicationUser : IdentityUser
{
    public string Password { get; set; }
    public string Name { get; set; }
    public string CreatorCode { get; set; }
    public bool IsActive { get; set; }
    public string Role { get; set; }
    public string RoleName { get; set; }
    public string Image { get; set; } = "-1";
}
