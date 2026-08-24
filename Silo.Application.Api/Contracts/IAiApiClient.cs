namespace Silo.Application.Api.Contracts;

public interface IAiApiClient
{
    /// <summary>Sends image bytes to the AI for OCR and returns the extracted text.</summary>
    Task<string> SendImageAsync(byte[] imageData, string mediaType, string promptKey, CancellationToken cancellationToken = default);
}
