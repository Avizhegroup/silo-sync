using System.Text.Json.Serialization;

namespace Silo.Application.Features;
public class GetCitiesVm
{
    public string Id { get; set; }
    public string Title { get; set; }
    public string ProvinceId { get; set; }
}
[JsonSerializable(typeof(ApiResponse<List<GetCitiesVm>>))]
[JsonSerializable(typeof(Dictionary<string, object>))]
[JsonSerializable(typeof(object))]
[JsonSerializable(typeof(string))]
public partial class GetCitiesVmContext : JsonSerializerContext
{

}
