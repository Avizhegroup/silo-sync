namespace Silo.Application.Features;

public class ChatHistoryDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public List<ChatMessageDto> Messages { get; set; } = new();
    public DateTime CreatedDate { get; set; }
    public DateTime LastUpdated { get; set; }
}
