namespace Silo.Application.Features;

public class AnalyzeProductImageCommand : IRequest<AnalyzeProductImageVm>
{
    /// <summary>Alternative to <see cref="Images"/>: use an image already stored in the gallery.</summary>
    public int GalleryId { get; set; }

    public List<ProductImageFileDto> Images { get; set; } = [];
}

public class ProductImageFileDto
{
    public string FileName { get; set; }
    public string ContentType { get; set; }
    public string Base64 { get; set; }
}
