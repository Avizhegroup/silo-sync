using System.Net;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Silo.Application.Contracts;
using Silo.Shared.Tools;

namespace Silo.Application.Api.Features;

public class SendChatMessageHandler(
    WmsApiContext context,
    ISiloAiClient siloAiClient,
    IDataAccess dataAccess,
    ILogger<SendChatMessageHandler> logger)
    : IRequestHandler<SendChatMessageCommand, SendChatMessageVm>
{
    private const string FallbackErrorMessage = ".متأسفانه در حال حاضر امکان پاسخگویی وجود ندارد. لطفاً کمی بعد دوباره تلاش کنید.";

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

        var result = await siloAiClient.SendAsync(state.ConversationId, request.Message, request.Mode, cancellationToken);

        if (result is null)
        {
            logger.LogWarning("Silo AI request failed for user {UserId}, sessionId {SessionId}", request.UserId, request.SessionId);

            return new SendChatMessageVm
            {
                ResponseText = FallbackErrorMessage,
                SessionId = request.SessionId
            };
        }

        if (result.StatusCode == HttpStatusCode.PaymentRequired)
        {
            return new SendChatMessageVm
            {
                StatusCode = 402,
                SessionId = request.SessionId
            };
        }

        var conversationId = result.ConversationId;

        var priceUsage = result.PriceUsage;

        var sqlCommands = new List<string>();

        var searchCommands = new List<string>();

        var configCommands = new List<string>();

        var cleanResponseText = AiCommandTools.StripSqlBlocks(result.ResponseText ?? string.Empty, sqlCommands);

        cleanResponseText = AiCommandTools.StripSearchBlocks(cleanResponseText, searchCommands);

        cleanResponseText = AiCommandTools.StripConfigBlocks(cleanResponseText, configCommands);

        var searchResults = new List<List<object>>();

        var sqlResults = new List<List<object>>();

        var searchResponseText = string.Empty;

        if (sqlCommands.Count > 0)
        {
            try
            {
                var data = DataTableTools.DataTableToObjects( dataAccess.SqlDataAdapter(sqlCommands[0]));

                sqlResults.Add(data);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex,"Failed to execute SQL command from AI response for user {UserId}", request.UserId);
            }
        }

        if (searchCommands.Count > 0)
        {
            var finalAnswerGenerated = false;

            try
            {
                var data = DataTableTools.DataTableToObjects(dataAccess.SqlDataAdapter(searchCommands[0]));

                searchResults.Add(data);

                var databaseResultsJson = JsonSerializer.Serialize(data);

                try
                {

                    var finalResult = await siloAiClient.SendFinalAnswerAsync(
                        conversationId,
                        request.Message,
                        databaseResultsJson,
                        request.Mode,
                        cancellationToken);

                    if (finalResult is not null &&
                        finalResult.StatusCode is null or HttpStatusCode.OK &&
                        finalResult.ResponseText.HasValue())
                    {
                        var finalSqlCommands = new List<string>();

                        var finalSearchCommands = new List<string>();

                        var finalConfigCommands = new List<string>();

                        var finalText = AiCommandTools.StripSqlBlocks(
                            finalResult.ResponseText,
                            finalSqlCommands);

                        finalText = AiCommandTools.StripSearchBlocks(
                            finalText,
                            finalSearchCommands);

                        finalText = AiCommandTools.StripConfigBlocks(
                            finalText,
                            finalConfigCommands);

                        if (finalText.HasValue())
                        {
                            cleanResponseText = finalText;

                            conversationId = finalResult.ConversationId;

                            finalAnswerGenerated = true;
                        }

                        if (finalResult.PriceUsage.HasValue)
                        {
                            priceUsage = (priceUsage ?? 0m) + finalResult.PriceUsage.Value;
                        }
                    }
                    else
                    {
                        logger.LogWarning("Silo AI final-answer generation returned no usable response for user {UserId}", request.UserId);
                    }
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex,"Silo AI final-answer generation failed after Search execution for user {UserId}", request.UserId);
                }

                if (data.Count == 0 && !finalAnswerGenerated)
                {
                    cleanResponseText = cleanResponseText.HasNoValue()
                        ? "نتیجه‌ای برای جستجو پیدا نشد."
                        : $"{cleanResponseText}\n\nنتیجه‌ای برای جستجو پیدا نشد.";
                }
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex,"Failed to execute Search command for user {UserId}", request.UserId);

                cleanResponseText = cleanResponseText.HasNoValue()
                    ? "اجرای جستجو با خطا مواجه شد."
                    : $"{cleanResponseText}\n\nاجرای جستجو با خطا مواجه شد.";
            }
        }

        if (searchResponseText.HasValue())
        {
            cleanResponseText = cleanResponseText.HasNoValue()
                ? searchResponseText
                : $"{cleanResponseText}\n\n{searchResponseText}";
        }

        if (configCommands.Count > 0)
        {
            logger.LogInformation(
                "Received {ConfigCommandCount} CONFIG command(s) from AI for user {UserId}",
                configCommands.Count,
                request.UserId);
        }

        state.ConversationId = conversationId;

        state.Messages.Add(new ChatMessageDto
        {
            Text = request.Message,
            IsUser = true,
            Datetime = DateTime.Now
        });

        state.Messages.Add(new ChatMessageDto
        {
            Text = cleanResponseText,
            IsUser = false,
            Datetime = DateTime.Now
        });

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
                    usage = JsonSerializer.Deserialize<ChatTokenUsageDto>(
                                existingSession.TokenUsage)
                            ?? new ChatTokenUsageDto();
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

        AiGeneratedQueries? aiQuery = null;

        if (sqlCommands.Count > 0)
        {
            aiQuery = new AiGeneratedQueries
            {
                UserId = request.UserId,
                SessionId = sessionId,
                QueryText = sqlCommands[0],
                CreatedDate = DateTime.Now
            };

            context.AiGeneratedQueries.Add(aiQuery);

            await context.SaveChangesAsync(cancellationToken);
        }

        return new SendChatMessageVm
        {
            ResponseText = cleanResponseText,
            SessionId = sessionId,
            SqlCommandsResults = sqlResults,
            QueryId = aiQuery?.Id,
            SearchCommandsResults = searchResults
        };
    }
}
