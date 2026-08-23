namespace Silo.Application.Features;
public class SaveNonDocFileCommand
{
    public string FileName { get; set; }
    public int Type { get; set; }
    public string? Data { get; set; }
}
