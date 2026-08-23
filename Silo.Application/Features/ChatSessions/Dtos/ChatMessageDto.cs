namespace Silo.Application.Features;

public class ChatMessageDto
{
    public string Text { get; set; }
    public bool IsUser { get; set; }
    public DateTime Datetime { get; set; }
}
