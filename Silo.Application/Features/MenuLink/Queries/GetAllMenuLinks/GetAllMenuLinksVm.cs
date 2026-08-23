using System.Text.Json.Serialization;

namespace Silo.Application.Features;
public class GetAllMenuLinksVm
{
    public int Id { get; set; }
    public string Title { get; set; }
    public int ParentId { get; set; }
    public int Level { get; set; }
    public string Url { get; set; }
    public string IconName { get; set; }
    public bool IsShown { get; set; }
    public bool IsDedicated { get; set; } = false;
}

[JsonSerializable(typeof(ApiResponse<List<GetAllMenuLinksVm>>))]
public partial class GetAllMenuLinksVmContext : JsonSerializerContext
{

}
