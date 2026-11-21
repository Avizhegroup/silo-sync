namespace Silo.Application.Features;

public class SaveTagForTeamYarPrintDto
{
    [Required]
    public string Title { get; set; }

    [Required]
    public string Value { get; set; }
}
