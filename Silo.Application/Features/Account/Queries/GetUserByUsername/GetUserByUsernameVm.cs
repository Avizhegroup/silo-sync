using System.Text.Json.Serialization;

namespace Silo.Application.Features;

public class GetUserByUsernameVm
{
    public string Id { get; set; }

    [JsonPropertyName("UserName")]
    public string Username { get; set; }

    [JsonPropertyName("Name")]
    public string PersianName { get; set; }
    public string Details { get; set; }
    public string Role { get; set; }
    public string RoleName { get; set; }
    public string Image { get; set; }
}

[JsonSerializable(typeof(ApiResponse<List<GetUserByUsernameVm>>))]
public partial class GetUserByUsernameVmContext : JsonSerializerContext
{

}
