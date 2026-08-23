using Silo.Application.Api.Contracts;
namespace Silo.Api.Services;

/// <summary>
/// HTTP client that calls Silo.Api.AI endpoints.
/// The X-Api-Key header and base address are configured at registration time in Program.Services.cs.
/// </summary>
public class AiApiHttpClient(HttpClient httpClient) : IAiApiClient
{
    public async Task<string> NewSessionAsync(List<string> promptKeys, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PostAsJsonAsync("api/ai/chat/new-session",
            new { PromptKeys = promptKeys }, cancellationToken);

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<NewSessionResponse>(cancellationToken: cancellationToken);

        return result?.SessionJson ?? string.Empty;
    }

    public async Task<SendChatResponse> SendMessageAsync(
    string? sessionJson,
    string message,
    string username,
    List<string> promptKeys,
    CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PostAsJsonAsync("api/ai/chat/send",
            new
            {
                SessionJson = sessionJson,
                Message = message,
                Username = username,
                PromptKeys = promptKeys
            }, cancellationToken);

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<SendChatResponse>(cancellationToken: cancellationToken);

        return new SendChatResponse
        {
            ResponseText = result?.ResponseText ?? string.Empty,
            UpdatedSessionJson = result?.UpdatedSessionJson ?? string.Empty,
            TokenUsage = result?.TokenUsage,
            PriceUsage = result?.PriceUsage
        };
    }

    public async Task<string> SendImageAsync(byte[] imageData, string mediaType, string promptKey, CancellationToken cancellationToken = default)
    {
        using var content = new MultipartFormDataContent();
        content.Add(new ByteArrayContent(imageData), "imageData", "image.bin");
        content.Add(new StringContent(mediaType), "mediaType");
        content.Add(new StringContent(promptKey ?? string.Empty), "promptKey");

        var response = await httpClient.PostAsync("api/ai/agent/ocr", content, cancellationToken);

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<OcrResponse>(cancellationToken: cancellationToken);

        return result?.ExtractedText ?? string.Empty;
    }
}
