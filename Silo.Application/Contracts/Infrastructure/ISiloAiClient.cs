namespace Silo.Application;

/// <summary>Client for the Silo AI RAG chat API.</summary>
public interface ISiloAiClient
{
    /// <summary>Starts a brand-new RAG conversation and returns the resulting conversation id, or null on failure.</summary>
    Task<Guid?> StartNewSessionAsync(CancellationToken cancellationToken);

    /// <summary>Sends a message to the Silo AI RAG chat endpoint and returns the answer and updated conversation id.</summary>
    Task<RagChatResponse?> SendAsync(Guid? conversationId, string message, CancellationToken cancellationToken);
}
