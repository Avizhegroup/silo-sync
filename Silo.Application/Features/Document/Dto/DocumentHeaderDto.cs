namespace Silo.Application.Features;

public class DocumentHeaderDto
{
    public int Id { get; set; }

    public string? Key { get; set; }

    public string? UserId { get; set; }

    public DocumentImportType ImportType { get; set; }

    public string? FileName { get; set; }

    public string DocumentType { get; set; }

    public DateTime? ImportDateTime { get; set; }

    public string? Description { get; set; }

    public int DocumentStatusId { get; set; }

    public string? HeaderData { get; set; }
    
    public List<DocumentItemDto> DocumentItems { get; set; }
}
