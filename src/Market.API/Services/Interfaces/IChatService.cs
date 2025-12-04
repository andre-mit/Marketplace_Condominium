using Market.SharedApplication.ViewModels.ChatViewModels;

namespace Market.API.Services.Interfaces;

public interface IChatService
{
    Task<Guid> CreateChatAsync(Guid userId, int productId,
        CancellationToken cancellationToken = default);
    Task<SyncChatsViewModel> SyncChatsAsync(Guid userId, DateTime? after,
        CancellationToken cancellationToken = default);

    Task<ChatMessageSyncViewModel> SendMessageAsync(Guid chatSessionId, Guid senderId, string message,
        CancellationToken cancellationToken = default);
}