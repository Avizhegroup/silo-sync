using System.Net.Http.Json;
using System.Text.Json;
using Silo.Bot.Support.Models.Bale;

namespace Silo.Bot.Support.Services;

/// <summary>Thin wrapper over the Bale Bot HTTP API using a named HttpClient ("BaleClient").</summary>
public class BaleBotClient 
{
    public const string HttpClientName = "BaleClient";

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly HttpClient _httpClient;
    private readonly ILogger<BaleBotClient> _logger;

    public BaleBotClient(IHttpClientFactory httpClientFactory, ILogger<BaleBotClient> logger)
    {
        _httpClient = httpClientFactory.CreateClient(HttpClientName);
        _logger = logger;
    }

    public async Task<List<BaleUpdate>> GetUpdatesAsync(long offset, int timeoutSeconds, CancellationToken cancellationToken)
    {
        var url = $"getUpdates?offset={offset}&timeout={timeoutSeconds}";
        try
        {
            var response = await _httpClient.GetFromJsonAsync<BaleApiResponse<List<BaleUpdate>>>(url, JsonOptions, cancellationToken);
            if (response?.Ok == true && response.Result != null)
                return response.Result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling Bale getUpdates (offset={Offset})", offset);
        }

        return new List<BaleUpdate>();
    }

    public async Task<bool> SendMessageAsync(long chatId, string text, CancellationToken cancellationToken)
    {
        var body = new BaleSendMessageRequest { ChatId = chatId, Text = text };
        try
        {
            var response = await _httpClient.PostAsJsonAsync("sendMessage", body, cancellationToken);
            if (response.IsSuccessStatusCode)
                return true;

            _logger.LogWarning(
                "Bale sendMessage failed for chat {ChatId} with status code {StatusCode}",
                chatId,
                (int)response.StatusCode);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception sending Bale message to chat {ChatId}", chatId);
        }

        return false;
    }
}
