namespace Silo.Application.Api.Contracts;

public interface IAiApiClient
{
    /// <summary>Sends image bytes to the AI for OCR and returns the extracted text.</summary>
    Task<string> SendIFileGetStringAsync(byte[] imageData
        , string mediaType
        , RagDocType docType
        , string key
        , CancellationToken cancellationToken = default);
}
