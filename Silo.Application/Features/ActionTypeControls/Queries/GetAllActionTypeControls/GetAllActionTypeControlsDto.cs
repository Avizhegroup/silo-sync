using System.Text.Json.Serialization;

namespace Silo.Application.Features;
public class GetAllActionTypeControlsDto
{
    public int Id { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_Code))]
    public string? Code { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_Name))]
    public string? Name { get; set; }

    [JsonIgnore]
    public bool IsChoosen { get; set; } = false;
}
