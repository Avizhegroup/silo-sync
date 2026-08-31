using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Silo.Application;

namespace Silo.Infrastructure.Shared;

/// <summary>Client that calls POST /api/rag/chat/send on the Silo AI API using a named HttpClient ("SiloAiClient").</summary>
public class SiloAiClient : ISiloAiClient
{
    public const string HttpClientName = "SiloAiClient";
    private const string SendEndpoint = "api/rag/chat/send";
    private const string NewSessionEndpoint = "api/rag/chat/new-session";

    private readonly HttpClient _httpClient;
    private readonly RagAiOptions _options;
    private readonly ILogger<SiloAiClient> _logger;

    public SiloAiClient(IHttpClientFactory httpClientFactory, IOptions<RagAiOptions> options, ILogger<SiloAiClient> logger)
    {
        _httpClient = httpClientFactory.CreateClient(HttpClientName);
        _options = options.Value;
        _logger = logger;
    }

    public async Task<Guid?> StartNewSessionAsync(CancellationToken cancellationToken)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync(NewSessionEndpoint, new { }, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning(
                    "Silo AI new-session request failed with status code {StatusCode}",
                    (int)response.StatusCode);
                return null;
            }

            var result = await response.Content.ReadFromJsonAsync<RagChatResponse>(cancellationToken: cancellationToken);
            return result?.ConversationId;
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception calling Silo AI new-session endpoint");
            return null;
        }
    }

    public async Task<RagChatResponse?> SendAsync(Guid? conversationId, string message, CancellationToken cancellationToken, RagDocType? docType = null)
    {
        var request = new RagChatRequest
        {
            ConversationId = conversationId,
            Message = message,
            TopK = _options.TopK,
            IsMainChat = true,
            DocType = docType ?? _options.DocType,
            Key = _options.Key
        };

        try
        {
            var response = await _httpClient.PostAsJsonAsync(SendEndpoint, request, cancellationToken);
           
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning(
                    "Silo AI request failed with status code {StatusCode}",
                    (int)response.StatusCode);

                return new RagChatResponse
                {
                    StatusCode = response.StatusCode
                };
            }

            return await response.Content.ReadFromJsonAsync<RagChatResponse>(cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception calling Silo AI RAG chat endpoint");

            return null;
        }
    }
}
