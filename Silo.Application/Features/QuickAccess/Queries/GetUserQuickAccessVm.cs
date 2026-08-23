using System.Text.Json.Serialization;

namespace Silo.Application.Features;

public class GetUserQuickAccessVm
{
    public int Id { get; set; }
    public int MenuLinkId { get; set; }
    public string Title { get; set; }
    public string Url { get; set; }
    public string IconName { get; set; }
}

[JsonSerializable(typeof(ApiResponse<List<GetUserQuickAccessVm>>))]
public partial class GetUserQuickAccessVmContext : JsonSerializerContext
{
}
