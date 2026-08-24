using System.Text.Json.Serialization;

namespace Silo.Application;

/// <summary>Response body from POST /api/rag/chat/send, matching Silo.Application.AI.Shared.RagChatResponse.</summary>
public class RagChatResponse
{
    [JsonPropertyName("responseText")]
    public string? ResponseText { get; set; }

    [JsonPropertyName("conversationId")]
    public Guid ConversationId { get; set; }

    [JsonPropertyName("citations")]
    public List<RagChatCitationDto> Citations { get; set; } = [];

    [JsonPropertyName("tokenUsage")]
    public ChatTokenUsageDto? TokenUsage { get; set; }

    [JsonPropertyName("priceUsage")]
    public decimal? PriceUsage { get; set; }
}
