namespace Silo.Application.Api.Contracts;

public interface IAiApiClient
{
    /// <summary>Creates a new AI agent session and returns the serialized session JSON.</summary>
    Task<string> NewSessionAsync(List<string> promptKeys, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a chat message to an existing (or new) session.
    /// Returns the AI response text and the updated serialized session JSON.
    /// </summary>
    Task<SendChatResponse> SendMessageAsync(
    string? sessionJson,
    string message,
    string username,
    List<string> promptKeys,
    CancellationToken cancellationToken = default);

    /// <summary>Sends image bytes to the AI for OCR and returns the extracted text.</summary>
    Task<string> SendImageAsync(byte[] imageData, string mediaType, string promptKey, CancellationToken cancellationToken = default);
}
