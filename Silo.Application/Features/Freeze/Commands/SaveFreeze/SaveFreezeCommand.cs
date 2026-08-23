namespace Silo.Application.Features;

public class SaveFreezeCommand
{
    public bool Status { get; set; } = true;

    [StringLength(256)]
    public string? Description { get; set; }

    public List<string> Serials { get; set; } = new();
}
