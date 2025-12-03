using Market.Domain.Entities;

namespace Market.Domain.Repositories;

// TODO: Add cancelation tokens
/// <summary>
/// Repository interface for managing chat sessions.
/// </summary>
public interface IChatSessionRepository
{
    /// <summary>
    /// Creates a new chat session between a customer and a seller for a specific product.
    /// </summary>
    /// <param name="productId"></param>
    /// <param name="customerId"></param>
    /// <returns>The ID of the newly created chat session.</returns>
    Task<Guid> CreateChatSessionAsync(int productId, Guid customerId);
    
    /// <summary>
    /// Retrieves a chat session by its ID.
    /// </summary>
    /// <param name="chatSessionId"></param>
    /// <returns>The chat session if found; otherwise, null.</returns>
    Task<ChatSession?> GetChatSessionByIdAsync(Guid chatSessionId);
    
    /// <summary>
    /// Retrieves all chat sessions associated with a specific user (either as a seller or customer).
    /// </summary>
    /// <param name="userId"></param>
    /// <returns>A collection of chat sessions.</returns>
    Task<IEnumerable<ChatSession>> GetChatSessionsByUserIdAsync(Guid userId);
    
    /// <summary>
    /// Retrieves chat sessions that have been updated since the last synchronization time for a specific user.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="lastSync"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>A collection of chat sessions updated since the last sync.</returns>
    Task<IEnumerable<ChatSession>> GetSyncChatSessionsAsync(Guid userId, DateTime lastSync, CancellationToken cancellationToken = default);
}