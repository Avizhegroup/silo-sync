namespace Silo.Application.Api.Features;

public class UpdateChatSessionsHandler(WmsApiContext context)
    : IRequestHandler<UpdateChatSessionsCommand, UpdateChatSessionsVm>
{
    public async Task<UpdateChatSessionsVm> Handle(UpdateChatSessionsCommand request, CancellationToken cancellationToken)
    {
        var existing = await context.ChatSessions
            .FirstOrDefaultAsync(s => s.SessionId == request.SessionId && s.UserId == request.UserId, cancellationToken);

        if (existing == null)
        {
            return new UpdateChatSessionsVm
            {
                Result = 0
            };
        }

        existing.SessionData = request.SessionData;

        if (request.TokenUsage is not null)
        {
            ChatTokenUsageDto usage;

            if (existing.TokenUsage.HasNoValue())
            {
                usage = new ChatTokenUsageDto();
            }
            else
            {
                usage = JsonSerializer.Deserialize<ChatTokenUsageDto>(existing.TokenUsage) ?? new ChatTokenUsageDto();
            }

            usage.InputTokenCount += request.TokenUsage.InputTokenCount;
            usage.OutputTokenCount += request.TokenUsage.OutputTokenCount;
            usage.CachedInputTokenCount += request.TokenUsage.CachedInputTokenCount;
            usage.TotalTokenCount += request.TokenUsage.TotalTokenCount;

            existing.TokenUsage = JsonSerializer.Serialize(usage);
        }

        existing.PriceUsage += request.PriceUsage;

        existing.LastUpdated = DateTime.Now;

        await context.SaveChangesAsync(cancellationToken);

        return new UpdateChatSessionsVm
        {
            Result = existing.SessionId
        };
    }
}
