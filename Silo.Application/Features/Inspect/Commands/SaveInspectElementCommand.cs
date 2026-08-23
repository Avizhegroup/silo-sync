using System.Text.Json.Serialization;

namespace Silo.Application.Features;

public class SaveInspectElementCommand
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_Field_Title))]
    [Required(ErrorMessageResourceType = typeof(TextResources), ErrorMessageResourceName = nameof(TextResources.APP_StringKeys_Validation_Required))]
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("inspectElementType")]
    public InspectElementType InspectElementType { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_Default_value))]
    [Required(ErrorMessageResourceType = typeof(TextResources), ErrorMessageResourceName = nameof(TextResources.APP_StringKeys_Validation_Required))]
    [JsonPropertyName("value")]
    public string Value { get; set; }

    [JsonPropertyName("minValue")]
    public int MinValue { get; set; }

    [JsonPropertyName("maxValue")]
    public int MaxValue { get; set; }

    [JsonPropertyName("prevent")]
    public bool Prevent { get; set; }

    [JsonPropertyName("isActive")]
    public bool IsActive { get; set; }

    [JsonPropertyName("isRequired")]
    public bool IsRequired { get; set; }

    [JsonPropertyName("productTypes")]
    public List<string> ProductTypes { get; set; } = new();

    [JsonPropertyName("options")]
    public List<string> Options { get; set; } = new();

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_InspectElement_Row))]
    [Range(1, 1000, ErrorMessageResourceType = typeof(TextResources), ErrorMessageResourceName = nameof(TextResources.APP_StringKeys_Validation_Required))]
    [JsonPropertyName("row")]
    public int Row { get; set; }
}

[JsonSerializable(typeof(ApiResponse<List<SaveInspectElementCommand>>))]
public partial class SaveInspectElementCommandContext : JsonSerializerContext
{
}
