using Market.SharedApplication.ViewModels.ChatViewModels;

namespace Market.API.Services;

public class ChatService(
    ILogger<ChatService> logger,
    IChatSessionRepository chatSessionRepository,
    IChatMessageRepository chatMessageRepository,
    IExpoNotificationService expoNotificationService) : IChatService
{
    public async Task<SyncChatsViewModel> SyncChatsAsync(Guid userId, DateTime? after,
        CancellationToken cancellationToken = default)
    {
        var afterFilled = after ?? DateTime.UtcNow.AddDays(-1);
        var chats = await chatSessionRepository.GetSyncChatSessionsAsync(userId, afterFilled, cancellationToken);

        var viewModel = new SyncChatsViewModel
        {
            ServerTime = DateTime.UtcNow,
            Chats = chats.Select(c =>
                {
                    var participant = c.BuyerId == userId ? c.Seller : c.Buyer!;
                    return new ChatSessionSyncViewModel
                    {
                        Id = c.Id,
                        ParticipantId = participant.Id,
                        ParticipantName = participant.FullName,
                        ParticipantAvatarUrl = participant.AvatarUrl,
                        ProductId = c.ProductId,
                        ProductName = c.Product!.Name,
                        ProductImageUrl = c.Product!.Images![0].Url,
                        CreatedAt = c.CreatedAt,
                        UpdatedAt = c.UpdatedAt,
                        LastMessageAt = c.UpdatedAt,
                        LastMessageContent = c.Messages?.OrderByDescending(m => m.SentAt).FirstOrDefault()?.Content,
                        UnreadCount = c.Messages?.Count ?? 0,
                        Messages = c.Messages?.Select(m => new ChatMessageSyncViewModel
                        {
                            Id = m.Id,
                            SenderId = m.SenderId,
                            Content = m.Content,
                            CreatedAt = m.SentAt,
                            ChatId = c.Id,
                            ProductId = c.ProductId,
                            SenderName = m.SenderId == c.BuyerId ? c.Buyer!.FullName : c.Seller?.FullName
                        }).ToList() ?? []
                    };
                }
            ).ToList()
        };

        return viewModel;
    }

    public async Task<ChatMessageSyncViewModel> SendMessageAsync(Guid chatSessionId, Guid senderId, string message,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var chatSession = await chatSessionRepository.GetChatSessionByIdAsync(chatSessionId);
            if (chatSession == null)
            {
                logger.LogWarning(
                    "User {UserId} attempted to send message to non-existent chat session {ChatSessionId}",
                    senderId, chatSessionId);

                throw new Exception("Chat session does not exist");
            }

            chatSession.UserIsParticipantInChat(senderId);

            await chatMessageRepository.AddMessageAsync(chatSessionId, senderId, message, cancellationToken);

            var receiver = chatSession.SellerId == senderId ? chatSession.Buyer! : chatSession.Seller;
            var sender = chatSession.SellerId == senderId ? chatSession.Seller : chatSession.Buyer!;

            var data = new ChatMessageSyncViewModel
            {
                ChatId = chatSessionId,
                SenderId = senderId,
                Content = message,
                SenderName = sender.FullName,
                CreatedAt = DateTime.UtcNow,
                ProductId = chatSession.ProductId
            };

            if (!string.IsNullOrEmpty(receiver.NotificationToken))
            {
                await expoNotificationService.SendNotificationAsync(
                    receiver.NotificationToken,
                    $"{sender.FirstName}",
                    message.Truncate(50),
                    data
                );
            }

            return data;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error sending message in chat session {ChatSessionId} by user {SenderId}",
                chatSessionId, senderId);
            throw;
        }
    }
}