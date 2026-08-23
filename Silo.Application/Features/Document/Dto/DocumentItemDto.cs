namespace Silo.Application.Features;

public class DocumentItemDto
{
    public int Id { get; set; }

    public string? Key { get; set; }

    public string DocumentType { get; set; }

    public string? ProductCode { get; set; }

    public string? ProductTitle { get; set; }

    public decimal Count { get; set; }

    public string? ProductUnit { get; set; }

    public string? ItemData { get; set; }
}
