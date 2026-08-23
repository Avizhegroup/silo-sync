using System.IO.Compression;

namespace Silo.Application.Api.Features;
public class GetAllGalleryImagesHandler (WmsApiContext apiContext) : IRequestHandler<GetAllGalleryImagesQuery, GetAllGalleryImagesVm>
{
    public async Task<GetAllGalleryImagesVm> Handle(GetAllGalleryImagesQuery request, CancellationToken cancellationToken)
    {
        List<string> paths = apiContext
            .GalleryMedias
            .Where(p=>p.UsageType > (int)GalleryUsageType.TruckCrossExit)
            .Select(p=>p.MediaPath)
            .ToList();

        List<string> validImagePaths = paths
          .Where(path => File.Exists(path))
          .ToList();

        using var memoryStream = new MemoryStream();
        using (var zipArchive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
        {
            foreach (string imagePath in validImagePaths)
            {
                string fileName = Path.GetFileName(imagePath);
                var entry = zipArchive.CreateEntry(fileName, CompressionLevel.Optimal);
                using var entryStream = entry.Open();
                using var fileStream = File.OpenRead(imagePath);
                await fileStream.CopyToAsync(entryStream, cancellationToken);
            }
        }

        memoryStream.Seek(0, SeekOrigin.Begin);

        return new GetAllGalleryImagesVm
        {
            Message = validImagePaths.Any() ? "Images zipped successfully." : "No valid images found.",
            ZipFile = memoryStream.ToArray()
        };
    }
}
