namespace Silo.Application.Features;
public class SaveDocumentLogCommand
{
    public List<DocumentKeyTypeDto> DocKeyTypes { get; set; }
    public int Status { get; set; }
    public DocumentEventType EventType { get; set; }
    public string UserId { get; set; }
    public string Description { get; set; }
}
