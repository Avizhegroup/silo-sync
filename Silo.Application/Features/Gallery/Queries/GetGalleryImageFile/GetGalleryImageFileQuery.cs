using MediatR;

namespace Silo.Application.Features;
public class GetGalleryImageFileQuery : IRequest<GetGalleryImageFileVm>
{
    public int Id { get; set; }
    public GalleryUsageType? UsageType { get; set; }
    public string? UsageId { get; set; }
}
