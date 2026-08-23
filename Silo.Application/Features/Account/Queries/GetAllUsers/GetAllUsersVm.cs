using System.Text.Json.Serialization;

namespace Silo.Application.Features;
public class GetAllUsersVm
{
    public Guid Id { get; set; }
    public string Username { get; set; }
    public string Name { get; set; }
    public bool IsActive { get; set; }
    public string Details { get; set; }
}

[JsonSerializable(typeof(ApiResponse<List<GetAllUsersVm>>))]
public partial class GetAllUsersVmContext : JsonSerializerContext
{

}
