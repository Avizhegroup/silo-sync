using Microsoft.Extensions.Logging;

namespace Silo.Application.Api.Features;

public class SendChatMessageHandler(
    WmsApiContext context,
    ISiloAiClient siloAiClient,
    ILogger<SendChatMessageHandler> logger)
    : IRequestHandler<SendChatMessageCommand, SendChatMessageVm>
{
    private const string FallbackErrorMessage = "متأسفانه در حال حاضر امکان پاسخگویی وجود ندارد. لطفاً کمی بعد دوباره تلاش کنید";

    public async Task<SendChatMessageVm> Handle(SendChatMessageCommand request, CancellationToken cancellationToken)
    {
        Silo.Domains.Entities.ChatSessions? existingSession = null;
        var state = new ChatSessionStateDto();

        if (request.SessionId != 0)
        {
            existingSession = await context.ChatSessions
                .FirstOrDefaultAsync(s => s.SessionId == request.SessionId && s.UserId == request.UserId, cancellationToken);

            if (existingSession?.SessionData.HasValue() == true)
            {
                try
                {
                    state = JsonSerializer.Deserialize<ChatSessionStateDto>(existingSession.SessionData) ?? state;
                }
                catch (JsonException)
                {
                    state = new ChatSessionStateDto();
                }
            }
        }

        var result = await siloAiClient.SendAsync(state.ConversationId, request.Message, cancellationToken, request.Mode);

        if (result is null)
        {
            logger.LogWarning("Silo AI request failed for user {UserId}, sessionId {SessionId}", request.UserId, request.SessionId);

            return new SendChatMessageVm
            {
                ResponseText = FallbackErrorMessage,
                SessionId = request.SessionId
            };
        }

        var priceUsage = result.PriceUsage;

        state.ConversationId = result.ConversationId;

        state.Messages.Add(new ChatMessageDto { Text = request.Message, IsUser = true, Datetime = DateTime.Now });
        state.Messages.Add(new ChatMessageDto { Text = result.ResponseText ?? string.Empty, IsUser = false, Datetime = DateTime.Now });

        var updatedJson = JsonSerializer.Serialize(state);

        int sessionId = request.SessionId;

        if (existingSession is null)
        {
            var chatSession = new Silo.Domains.Entities.ChatSessions
            {
                UserId = request.UserId,
                SessionData = updatedJson,
                TokenUsage = JsonSerializer.Serialize(result.TokenUsage ?? new ChatTokenUsageDto()),
                PriceUsage = priceUsage,
                Mode = (int)request.Mode,
                CreatedDate = DateTime.Now,
                LastUpdated = DateTime.Now
            };

            context.ChatSessions.Add(chatSession);

            await context.SaveChangesAsync(cancellationToken);

            sessionId = chatSession.SessionId;
        }
        else
        {
            existingSession.SessionData = updatedJson;

            if (result.TokenUsage is not null)
            {
                ChatTokenUsageDto usage;

                if (existingSession.TokenUsage.HasNoValue())
                {
                    usage = new ChatTokenUsageDto();
                }
                else
                {
                    usage = JsonSerializer.Deserialize<ChatTokenUsageDto>(existingSession.TokenUsage) ?? new ChatTokenUsageDto();
                }

                usage.InputTokenCount += result.TokenUsage.InputTokenCount;
                usage.OutputTokenCount += result.TokenUsage.OutputTokenCount;
                usage.CachedInputTokenCount += result.TokenUsage.CachedInputTokenCount;
                usage.TotalTokenCount += result.TokenUsage.TotalTokenCount;

                existingSession.TokenUsage = JsonSerializer.Serialize(usage);
            }

            existingSession.PriceUsage += priceUsage;

            existingSession.LastUpdated = DateTime.Now;

            await context.SaveChangesAsync(cancellationToken);
        }

        return new SendChatMessageVm
        {
            ResponseText = result.ResponseText ?? string.Empty,
            SessionId = sessionId
        };
    }
}
