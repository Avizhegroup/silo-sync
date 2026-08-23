using System.Text.Json.Serialization;

namespace Silo.Application.Features;
public class GetAllProvinceVm
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; }
}
[JsonSerializable(typeof(ApiResponse<List<GetAllProvinceVm>>))]
[JsonSerializable(typeof(Dictionary<string, object>))]
[JsonSerializable(typeof(object))]
[JsonSerializable(typeof(string))]
public partial class GetAllProvinceVmContext : JsonSerializerContext
{
}
