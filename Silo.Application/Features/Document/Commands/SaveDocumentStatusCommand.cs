namespace Silo.Application.Features;
public class SaveDocumentStatusCommand
{
    public List<DocumentKeyTypeDto> DocumentKeyTypes { get; set; } = new();
    public string User { get; set; }
    public int NewStatus { get; set; } = 0;
    public string Description { get; set; }
}
