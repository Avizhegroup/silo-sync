namespace Silo.Application.Dto;

public class UpdateUserCommand
{
    public string Id { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string LastModifier { get; set; }
    public bool IsActive { get; set; }
    public string PersianName { get; set; }
    public string Details { get; set; }
    public string Role { get; set; }
    public string Image { get; set; } = "-1";
}

public class ApiAuthenticateDto
{
    public string Username { get; set; }
    public string Password { get; set; }
    public string? StationMac { get; set; }
}
