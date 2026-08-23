using Microsoft.AspNetCore.Http;

namespace Silo.Application.Features;
public class SaveGalleryMediaWithFileCommand : IRequest<SaveGalleryMediaWithFileVm>
{
    public IFormFile File { get; set; }
    public string UserId { get; set; }
    public string MediaName { get; set; }
    public GalleryUsageType UsageType { get; set; }
    public string UsageId { get; set; }
    public GalleryExtension Extension { get; set; }
}
