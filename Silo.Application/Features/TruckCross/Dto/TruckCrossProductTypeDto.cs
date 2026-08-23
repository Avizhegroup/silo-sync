namespace Silo.Application.Features;
public  class TruckCrossProductTypeDto
{
    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_Code))]
    public int Id { get; set; }

    [Required]
    [StringLength(256)]
    public string Title { get; set; }

    public string? TruckCrossCauseIdsArray { get; set; }

    public List<int> TruckCrossCauseIds { get; set; }
}

