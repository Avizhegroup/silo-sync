namespace Silo.Application.Features;

public class AnalyzeProductImageCommand : IRequest<AnalyzeProductImageVm>
{
    /// <summary>Image already stored in the gallery (the file is read from disk, nothing is re-uploaded).</summary>
    public int GalleryId { get; set; }
}
