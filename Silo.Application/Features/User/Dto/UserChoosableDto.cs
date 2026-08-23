namespace Silo.Application.Features;
public class UserChoosableDto
{
    public string Id { get; set; }
    public string UserName { get; set; }
    public string Name { get; set; }
    public bool IsChoosed { get; set; } = false;
}
