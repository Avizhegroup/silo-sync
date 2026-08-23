namespace Silo.Application.Api.Features;

public class GetChatHistoriesHandler(WmsApiContext context)
    : IRequestHandler<GetChatHistoriesQuery, GetChatHistoriesVm>
{
    public async Task<GetChatHistoriesVm> Handle(GetChatHistoriesQuery request, CancellationToken cancellationToken)
    {
        var query = context.ChatSessions.Where(s => s.UserId == request.UserId);

        if (request.Mode is not null)
        {
            var mode = (int)request.Mode.Value;

            query = query.Where(s => s.Mode == mode);
        }

        var sessions = await query
            .OrderByDescending(s => s.LastUpdated)
            .ToListAsync(cancellationToken);

        var histories = sessions.Select(session =>
        {
            var state = ParseState(session.SessionData);

            var messages = state.Messages;

            var firstUserMessage = messages.FirstOrDefault(m => m.IsUser);

            var title = firstUserMessage is null
                ? "گفتگوی جدید"
                : firstUserMessage.Text.Length > 50
                    ? firstUserMessage.Text.Substring(0, 50) + "..."
                    : firstUserMessage.Text;

            return new ChatHistoryDto
            {
                Id = session.SessionId,
                Title = title,
                Messages = messages,
                CreatedDate = session.CreatedDate,
                LastUpdated = session.LastUpdated
            };
        }).ToList();

        return new GetChatHistoriesVm
        {
            Histories = histories
        };
    }

    private static ChatSessionStateDto ParseState(string? sessionData)
    {
        if (sessionData.HasNoValue())
        {
            return new ChatSessionStateDto();
        }

        try
        {
            return JsonSerializer.Deserialize<ChatSessionStateDto>(sessionData) ?? new ChatSessionStateDto();
        }
        catch (JsonException)
        {
            return new ChatSessionStateDto();
        }
    }
}
