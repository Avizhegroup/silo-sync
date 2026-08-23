namespace Silo.Application.Features;
public class RevokeDocumentAggregateCommand
{
    public List<DocumentKeyTypeDto> DocumentKeyTypes { get; set; }
    public int DocumentStatus { get; set; }
    public string Description { get; set; }
}
