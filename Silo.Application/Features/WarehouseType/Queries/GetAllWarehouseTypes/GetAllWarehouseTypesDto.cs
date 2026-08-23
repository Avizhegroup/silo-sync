using System.Text.Json.Serialization;

namespace Silo.Application.Features;
public class GetAllWarehouseTypesDto
{
    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_Code))]
    public string? Code { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_Title))]
    public string? Title { get; set; }

    [JsonIgnore]
    public bool IsChoosen { get; set; } = false;
}
