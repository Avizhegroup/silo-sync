namespace Silo.Application.Features;
public class GetOcrDataForGalleryMediaQuery : IRequest<GetOcrDataForGalleryMediaVm>
{
    public int GalleryId { get; set; }
    public GalleryOcrTypes OcrType { get; set; }
}
