using System.Collections.Concurrent;
using Microsoft.Extensions.Options;
using Silo.Bot.Support.Configuration;
using Silo.Bot.Support.Models.Bale;
using Silo.Bot.Support.Services;

namespace Silo.Bot.Support.Workers;

/// <summary>
/// Long-polls the Bale Bot API for updates, forwards each user text message to the
/// Silo AI RAG endpoint, and sends the AI answer back to the originating Bale chat.
/// </summary>
public class BaleBotWorker : BackgroundService
{
    private const string FallbackErrorMessage =
        "متأسفانه در حال حاضر امکان پاسخگویی وجود ندارد. لطفاً کمی بعد دوباره تلاش کنید.";

    /// <summary>Maximum number of updates processed concurrently across all chats.</summary>
    private const int MaxConcurrentUpdates = 10;

    private readonly SemaphoreSlim _globalSemaphore = new(MaxConcurrentUpdates, MaxConcurrentUpdates);
    private readonly ConcurrentDictionary<long, SemaphoreSlim> _perChatSemaphores = new();
    private readonly ConcurrentDictionary<long, Guid?> _chatConversations = new();

    private readonly BaleBotClient _baleClient;
    private readonly ISiloAiClient _siloAiClient;
    private readonly BaleOptions _options;
    private readonly ILogger<BaleBotWorker> _logger;

    public BaleBotWorker(
        BaleBotClient baleClient,
        ISiloAiClient siloAiClient,
        IOptions<BaleOptions> options,
        ILogger<BaleBotWorker> logger)
    {
        _baleClient = baleClient;
        _siloAiClient = siloAiClient;
        _options = options.Value;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Bale bot worker started");

        long offset = 0;

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                _logger.LogInformation("Polling Bale updates");

                var updates = await _baleClient.GetUpdatesAsync(
                    offset, _options.LongPollTimeoutSeconds, stoppingToken);

                // Advance the offset before dispatching processing so a failing
                // update is never fetched and processed again.
                foreach (var update in updates)
                {
                    if (update.UpdateId >= offset)
                        offset = update.UpdateId + 1;
                }

                var tasks = new List<Task>(updates.Count);
                foreach (var update in updates)
                    tasks.Add(ProcessUpdateAsync(update, stoppingToken));

                await Task.WhenAll(tasks);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while polling Bale updates; retrying in 5 seconds");
                try
                {
                    await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }
        }

        _logger.LogInformation("Bale bot worker stopped");
    }

    private async Task ProcessUpdateAsync(BaleUpdate update, CancellationToken stoppingToken)
    {
        _logger.LogInformation("Received Bale update {UpdateId}", update.UpdateId);

        var chat = update.Message?.Chat;
        var text = update.Message?.Text;

        if (chat is null || string.IsNullOrWhiteSpace(text))
            return;

        var chatId = chat.Id;
        var chatSemaphore = _perChatSemaphores.GetOrAdd(chatId, _ => new SemaphoreSlim(1, 1));

        await _globalSemaphore.WaitAsync(stoppingToken);
        try
        {
            await chatSemaphore.WaitAsync(stoppingToken);
            try
            {
                await HandleMessageAsync(update.UpdateId, chatId, text, stoppingToken);
            }
            finally
            {
                chatSemaphore.Release();
            }
        }
        finally
        {
            _globalSemaphore.Release();
        }
    }

    private async Task HandleMessageAsync(long updateId, long chatId, string text, CancellationToken stoppingToken)
    {
        _logger.LogInformation("Processing message for chat {ChatId}", chatId);

        _chatConversations.TryGetValue(chatId, out var conversationId);

        if (conversationId is null)
        {
            _logger.LogInformation("Starting new Silo AI conversation for chat {ChatId}", chatId);
            conversationId = await _siloAiClient.StartNewSessionAsync(stoppingToken);
            if (conversationId is not null)
                _chatConversations[chatId] = conversationId;
        }

        _logger.LogInformation("Sending request to Silo AI");
        var response = await _siloAiClient.SendAsync(conversationId, text, stoppingToken);

        if (response is null || string.IsNullOrWhiteSpace(response.ResponseText))
        {
            _logger.LogWarning(
                "Silo AI request failed for update {UpdateId} in chat {ChatId}", updateId, chatId);

            _chatConversations.TryRemove(chatId, out _);
            await _baleClient.SendMessageAsync(chatId, FallbackErrorMessage, stoppingToken);
            return;
        }

        _logger.LogInformation("Received Silo AI response");
        _chatConversations[chatId] = response.ConversationId;

        _logger.LogInformation("Sending response to Bale chat {ChatId}", chatId);
        var sent = await _baleClient.SendMessageAsync(chatId, response.ResponseText, stoppingToken);
        if (!sent)
        {
            _logger.LogWarning(
                "Failed to send Bale message for update {UpdateId} in chat {ChatId}", updateId, chatId);
        }
    }
}
