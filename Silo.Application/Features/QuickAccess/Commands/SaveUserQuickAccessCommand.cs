using System.Text.Json.Serialization;

namespace Silo.Application.Features;

public class SaveUserQuickAccessCommand
{
    public int MenuLinkId { get; set; }
}

[JsonSerializable(typeof(ApiResponse<bool>))]
public partial class SaveUserQuickAccessCommandContext : JsonSerializerContext
{
}

public class RemoveUserQuickAccessCommand
{
    public int Id { get; set; }
}

[JsonSerializable(typeof(ApiResponse<bool>))]
public partial class RemoveUserQuickAccessCommandContext : JsonSerializerContext
{
}
