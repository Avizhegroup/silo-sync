using Silo.Application.Api.Contracts;
namespace Silo.Api.Services;

/// <summary>
/// HTTP client that calls Silo.Api.AI endpoints.
/// The X-Api-Key header and base address are configured at registration time in Program.Services.cs.
/// </summary>
public class AiApiHttpClient(HttpClient httpClient) : IAiApiClient
{
    public async Task<string> SendIFileGetStringAsync(byte[] imageData
        , string mediaType
        , RagDocType docType
        , string key
        , CancellationToken cancellationToken = default)
    {
        using var content = new MultipartFormDataContent();
        content.Add(new ByteArrayContent(imageData), "imageData", "image.bin");
        content.Add(new StringContent(mediaType), "mediaType");
        content.Add(new StringContent(((int)docType).ToString()), "docType");
        content.Add(new StringContent(key), "key");

        var response = await httpClient.PostAsync("api/image/ocr", content, cancellationToken);

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<OcrResponse>(cancellationToken: cancellationToken);

        return result?.ExtractedText ?? string.Empty;
    }

    public async Task<string> AnalyzeImagesAsync(IReadOnlyList<AiImageInput> images
        , string prompt
        , string key = null
        , CancellationToken cancellationToken = default)
    {
        using var content = new MultipartFormDataContent();

        foreach (var image in images)
        {
            var part = new ByteArrayContent(image.Data);
            part.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(image.MediaType);
            content.Add(part, "images", image.FileName ?? "image");
        }

        if (!string.IsNullOrWhiteSpace(prompt))
        {
            content.Add(new StringContent(prompt), "prompt");
        }

        if (!string.IsNullOrWhiteSpace(key))
        {
            content.Add(new StringContent(key), "key");
        }

        content.Add(new StringContent(Guid.NewGuid().ToString()), "requestId");

        var response = await httpClient.PostAsync("api/image-analysis", content, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var status = (int)response.StatusCode;

            if (status is 400 or 402 or 422)
            {
                string message = null;

                try
                {
                    message = (await response.Content.ReadFromJsonAsync<AnalysisError>(cancellationToken: cancellationToken))?.Message;
                }
                catch (Exception)
                {
                    // body is not JSON; fall back to generic message
                }

                throw new AiImageAnalysisException(message ?? $"Image analysis failed ({status}).", status);
            }

            response.EnsureSuccessStatusCode();
        }

        var result = await response.Content.ReadFromJsonAsync<AnalysisResponse>(cancellationToken: cancellationToken);

        return result?.Text ?? string.Empty;
    }

    private sealed class AnalysisResponse
    {
        public string? Text { get; set; }
    }

    private sealed class AnalysisError
    {
        public string? Message { get; set; }
    }

    private sealed class OcrResponse
    {
        public string? ExtractedText { get; set; }
    }
}
