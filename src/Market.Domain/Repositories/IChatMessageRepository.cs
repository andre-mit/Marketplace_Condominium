using Market.Domain.Entities;

namespace Market.Domain.Repositories;

public interface IChatMessageRepository
{
    Task AddMessageAsync(Guid chatSessionId, Guid senderId, string message, CancellationToken cancellationToken = default);
    Task<IEnumerable<ChatMessage>> GetMessagesByChatSessionIdAsync(Guid chatSessionId);
}