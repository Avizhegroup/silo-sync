namespace Silo.Application.Features;

public class GalleryOcrExtractedTextDto
{
    public string ExtractedText { get; set; }
    public GalleryOcrTypes OcrType { get; set; }
    public int MediaId { get; set; }
}
