namespace Silo.Application.Api.Contracts;

public interface IAiApiClient
{
    /// <summary>Sends image bytes to the AI for OCR and returns the extracted text.</summary>
    Task<string> SendIFileGetStringAsync(byte[] imageData
        , string mediaType
        , RagDocType docType
        , string key
        , CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends 1-8 images to <c>POST /api/image-analysis</c> and returns the AI text answer.
    /// Throws <see cref="AiImageAnalysisException"/> when the AI service rejects the request (400/402/422).
    /// </summary>
    Task<string> AnalyzeImagesAsync(IReadOnlyList<AiImageInput> images
        , string prompt
        , string key = null
        , CancellationToken cancellationToken = default);
}

public record AiImageInput(byte[] Data, string MediaType, string FileName);

public class AiImageAnalysisException(string message, int statusCode) : Exception(message)
{
    public int StatusCode { get; } = statusCode;
}
