namespace Silo.Application.Features;

public class UpdateUserByIdCommand
{
    public string Id { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public bool IsActive { get; set; } = true;
    public string PersianName { get; set; }
    public string Role { get; set; }
    public string RoleName { get; set; }
    public string Details { get; set; }
    public string Image { get; set; } = "-1";
}
