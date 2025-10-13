using Market.Domain.Entities;

namespace Market.Domain.Repositories;

public interface IChatMessageRepository
{
    Task AddMessageAsync(Guid chatSessionId, Guid senderId, string message);
    Task<IEnumerable<ChatMessage>> GetMessagesByChatSessionIdAsync(Guid chatSessionId);
}