namespace Silo.Application.Features;
public class SaveGalleryMediaWithFileVm
{
    public int Id { get; set; }
    public string UserId { get; set; }
    public string MediaName { get; set; }
    public string MediaPath { get; set; }
    public GalleryUsageType UsageType { get; set; }
    public DateTime UpldoadDateTime { get; set; }
    public string UsageId { get; set; }
    public GalleryExtension Extension { get; set; }
}
