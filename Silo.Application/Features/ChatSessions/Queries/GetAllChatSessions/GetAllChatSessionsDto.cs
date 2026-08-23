
namespace Silo.Application.Features;

public class GetAllChatSessionsDto
{
    public int SessionId { get; set; }
    public string? UserId { get; set; }
    public string? SessionData { get; set; }
    public RagDocType? Mode { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime LastUpdated { get; set; }
}
