using Microsoft.Extensions.Logging;
using Silo.Application.Api.Contracts;
using Silo.Application.Contracts;
using Silo.Shared.Tools;

namespace Silo.Application.Api.Features;

public class SendChatMessageHandler(
    WmsApiContext context,
    IAiApiClient aiApiClient,
    IDataAccess dataAccess,
    ILogger<SendChatMessageHandler> logger)
    : IRequestHandler<SendChatMessageCommand, SendChatMessageVm>
{
    public async Task<SendChatMessageVm> Handle(SendChatMessageCommand request, CancellationToken cancellationToken)
    {
        Silo.Domains.Entities.ChatSessions? existingSession = null;
        string? sessionJson = null;

        if (request.SessionId != 0)
        {
            existingSession = await context.ChatSessions
                .FirstOrDefaultAsync(s => s.SessionId == request.SessionId && s.UserId == request.UserId, cancellationToken);

            sessionJson = existingSession?.SessionData;
        }

        var result = await aiApiClient.SendMessageAsync(
           sessionJson,
           request.Message,
           "کاربر",
          request.PromptKeys,
         cancellationToken);

        var responseText = result.ResponseText;
        var updatedJson = result.UpdatedSessionJson;
        var tokenUsage = result.TokenUsage;
        var priceUsage = result.PriceUsage;

        // Strip SQL blocks from the AI response and collect the SQL commands.
        // updatedJson (saved to DB) keeps the raw response for session restoration;
        // the cleaned text is returned to the UI.
        var sqlCommands = new List<string>();
        var cleanResponseText = SqlTextTools.StripSqlBlocks(responseText, sqlCommands);

        // Execute the first extracted SQL command and return results to the UI.
        var sqlResults = new List<List<object>>();
        if (sqlCommands.Count > 0)
        {
            try
            {
                var data = DataTableTools.DataTableToObjects(dataAccess.SqlDataAdapter(sqlCommands[0]));
                sqlResults.Add(data);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Failed to execute SQL command from AI response for user {UserId}", request.UserId);
            }
        }

        int sessionId = request.SessionId;

        if (sessionId == 0)
        {
            var chatSession = new Silo.Domains.Entities.ChatSessions
            {
                UserId = request.UserId,
                SessionData = updatedJson,
                TokenUsage = JsonSerializer.Serialize(tokenUsage ?? new ChatTokenUsageDto()),
                PriceUsage = priceUsage,
                Mode = (int)request.Mode,
                CreatedDate = DateTime.Now,
                LastUpdated = DateTime.Now
            };

            context.ChatSessions.Add(chatSession);

            await context.SaveChangesAsync(cancellationToken);

            sessionId = chatSession.SessionId;
        }
        else if (existingSession is not null)
        {
            existingSession.SessionData = updatedJson;

            if (tokenUsage is not null)
            {
                ChatTokenUsageDto usage;

                if (existingSession.TokenUsage.HasNoValue())
                {
                    usage = new ChatTokenUsageDto();
                }
                else
                {
                    usage = JsonSerializer.Deserialize<ChatTokenUsageDto>(existingSession.TokenUsage)  ?? new ChatTokenUsageDto();
                }

                usage.InputTokenCount += tokenUsage.InputTokenCount;
                usage.OutputTokenCount += tokenUsage.OutputTokenCount;
                usage.CachedInputTokenCount += tokenUsage.CachedInputTokenCount;
                usage.TotalTokenCount += tokenUsage.TotalTokenCount;

                existingSession.TokenUsage = JsonSerializer.Serialize(usage);
            }

            existingSession.PriceUsage += priceUsage;

            existingSession.LastUpdated = DateTime.Now;
            await context.SaveChangesAsync(cancellationToken);
        }

        return new SendChatMessageVm
        {
            ResponseText = cleanResponseText,
            SessionId = sessionId,
            SqlCommandsResults = sqlResults
        };
    }
}
