using Silo.Application.Api.Contracts;
namespace Silo.Api.Services;

/// <summary>
/// HTTP client that calls Silo.Api.AI endpoints.
/// The X-Api-Key header and base address are configured at registration time in Program.Services.cs.
/// </summary>
public class AiApiHttpClient(HttpClient httpClient) : IAiApiClient
{
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

    private sealed class OcrResponse
    {
        public string? ExtractedText { get; set; }
    }
}
