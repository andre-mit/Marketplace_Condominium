using Market.API.Hubs;
using Market.SharedApplication.ViewModels.ChatViewModels;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Distributed;
using static Market.API.Helpers.Constants;

namespace Market.API.Services;

public class ChatService(
    ILogger<ChatService> logger,
    IChatSessionRepository chatSessionRepository,
    IChatMessageRepository chatMessageRepository,
    IExpoNotificationService expoNotificationService,
    IHubContext<ChatHub> hubContext,
    IDistributedCache cache) : IChatService
{
    public async Task<ChatSessionSyncViewModel> GetChatSessionAsync(Guid chatSessionId, Guid userId,
        CancellationToken cancellationToken = default)
    {
        var chatSession = await chatSessionRepository.GetChatSessionByIdAsync(chatSessionId, cancellationToken);
        if (chatSession == null)
        {
            logger.LogWarning("User {UserId} attempted to access non-existent chat session {ChatSessionId}",
                userId, chatSessionId);

            throw new Exception("Chat session does not exist");
        }

        chatSession.UserIsParticipantInChat(userId);
        
        var participant = chatSession.BuyerId == userId ? chatSession.Seller : chatSession.Buyer!;
        
        var viewModel = new ChatSessionSyncViewModel
        {
            Id = chatSession.Id,
            ParticipantId = participant.Id,
            ParticipantName = participant.FullName,
            ParticipantAvatarUrl = participant.AvatarUrl,
            ProductId = chatSession.ProductId,
            ProductName = chatSession.Product!.Name,
            ProductImageUrl = chatSession.Product!.Images![0].Url,
            CreatedAt = chatSession.CreatedAt,
            UpdatedAt = chatSession.UpdatedAt,
            LastMessageAt = chatSession.UpdatedAt,
            LastMessageContent = chatSession.Messages?.OrderByDescending(m => m.SentAt).FirstOrDefault()?.Content,
            UnreadCount = chatSession.Messages?.Count ?? 0,
            Messages = chatSession.Messages?.Select(m => new ChatMessageSyncViewModel
            {
                Id = m.Id,
                SenderId = m.SenderId,
                Content = m.Content,
                CreatedAt = m.SentAt,
                ChatId = chatSession.Id,
                ProductId = chatSession.ProductId,
                SenderName = m.SenderId == chatSession.BuyerId ? chatSession.Buyer!.FullName : chatSession.Seller?.FullName
            }).ToList() ?? []
        };
        
        return viewModel;
    }
    
    public async Task<Guid> CreateChatAsync(Guid userId, int productId,
        CancellationToken cancellationToken = default)
    {
        var chatSessionId = await chatSessionRepository.CreateChatSessionAsync(productId, userId, cancellationToken);

        logger.LogInformation("Chat session {ChatSessionId} created by user {UserId} for product {ProductId}",
            chatSessionId, userId, productId);

        return chatSessionId;
    }

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
            var chatSession = await chatSessionRepository.GetChatSessionByIdAsync(chatSessionId, cancellationToken);
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
                ProductId = chatSession.ProductId,
                SenderAvatarUrl = sender.AvatarUrl
            };
            
            var userOnlineConnectionId = await GetUserOnlineConnectionIdAsync(receiver.Id);
            if (userOnlineConnectionId != null)
            {
                await hubContext.Clients.Client(userOnlineConnectionId).SendAsync("ReceiveMessage", data, cancellationToken);
            }

            if (!string.IsNullOrEmpty(receiver.NotificationToken))
            {
                await expoNotificationService.SendNotificationAsync(
                    receiver.NotificationToken,
                    $"{sender.FirstName}",
                    message.Truncate(50),
                    data
                , cancellationToken);
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
    
    private async Task<string?> GetUserOnlineConnectionIdAsync(Guid userId)
    {
        var key = Constants.GetOnlineUserCacheKey(userId);
        var connectionId = await cache.GetStringAsync(key);
        return connectionId;
    }
}