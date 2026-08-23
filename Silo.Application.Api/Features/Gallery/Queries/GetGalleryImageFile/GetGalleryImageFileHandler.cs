namespace Silo.Application.Api.Features;
public class GetGalleryImageFileHandler(WmsApiContext apiContext) : IRequestHandler<GetGalleryImageFileQuery, GetGalleryImageFileVm>
{
    public async Task<GetGalleryImageFileVm> Handle(GetGalleryImageFileQuery request, CancellationToken cancellationToken)
    {
        var query = apiContext.GalleryMedias.AsQueryable();

        if (request.Id > 0)
        {
            query = query.Where(p => p.Id == request.Id);
        }

        if (request.UsageType is not null)
        {
            query = query.Where(p => p.UsageType == (int)request.UsageType);
        }

        if (request.UsageId.HasValue())
        {
            query = query.Where(p => p.UsageId == request.UsageId);
        }

        var galleryMedia = await query.FirstOrDefaultAsync(cancellationToken);

        if (galleryMedia is null || !File.Exists(galleryMedia.MediaPath))
        {
            return new GetGalleryImageFileVm();
        }

        byte[] imageBytes = await File.ReadAllBytesAsync(galleryMedia.MediaPath, cancellationToken);

        return new GetGalleryImageFileVm()
        {
            ImageFile = imageBytes
        };
    }
}
