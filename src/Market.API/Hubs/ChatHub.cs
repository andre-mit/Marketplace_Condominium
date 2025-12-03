using Market.Domain.Repositories;
using Market.SharedApplication.ViewModels.ChatViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Distributed;

namespace Market.API.Hubs;

[Authorize]
public class ChatHub(
    ILogger<ChatHub> logger,
    IChatSessionRepository chatSessionRepository,
    IChatMessageRepository chatMessageRepository,
    IChatService chatService,
    IDistributedCache cache) : Hub
{
    private const string OnlineUserCacheKey = "chat:online";
    private const string InChatUserCacheKey = "chat:inchat";

    /// <summary>
    /// Sends a message to a chat session.
    /// </summary>
    /// <param name="chatSessionId"></param>
    /// <param name="message"></param>
    /// <exception cref="HubException"></exception>
    [HubMethodName("SendMessage")]
    public async Task SendMessage(Guid chatSessionId, string message, CancellationToken cancellationToken =default)
    {
        var userId = Guid.Parse(Context.UserIdentifier!);
        
        var data = chatService.SendMessageAsync(chatSessionId, userId, message);
        
        await Clients.Group(chatSessionId.ToString()).SendAsync("ReceiveMessage", data);
        
        
    }
    
    private async Task SendMessageMigrateToService(Guid chatSessionId, Guid senderId, string message)
    {
        var chatSession = await chatSessionRepository.GetChatSessionByIdAsync(chatSessionId);
        if (chatSession == null)
        {
            logger.LogWarning("User {UserId} attempted to send message to non-existent chat session {ChatSessionId}",
                senderId, chatSessionId);
            throw new HubException("Chat session does not exist");
        }
        
        EnsureUserIsParticipantInChat(senderId, chatSession);
    }

    /// <summary>
    /// Joins a chat session.
    /// </summary>
    /// <param name="chatSessionId"></param>
    /// <returns>If other yser is online</returns>
    /// <exception cref="HubException"></exception>
    [HubMethodName("JoinChatSession")]
    public async Task<bool> JoinChatSession(Guid chatSessionId)
    {
        var userId = Guid.Parse(Context.UserIdentifier!);
        var chatSession = await chatSessionRepository.GetChatSessionByIdAsync(chatSessionId);
        if (chatSession == null)
        {
            logger.LogWarning("User {UserId} attempted to join non-existent chat session {ChatSessionId}", userId,
                chatSessionId);
            throw new HubException("Chat session does not exist");
        }

        EnsureUserIsParticipantInChat(userId, chatSession);

        await Groups.AddToGroupAsync(Context.ConnectionId, chatSessionId.ToString());
        var key = GetInChatUserCacheKey(userId, chatSessionId);
        await cache.SetStringAsync(key, "true");

        var otherUserId = chatSession.BuyerId == userId ? chatSession.SellerId : chatSession.BuyerId;

        return await IsUserOnlineAsync(otherUserId);
    }

    /// <summary>
    /// Leaves a chat session.
    /// </summary>
    /// <param name="chatSessionId"></param>
    /// <returns></returns>
    /// <exception cref="HubException"></exception>
    [HubMethodName("LeaveChatSession")]
    public async Task LeaveChatSession(Guid chatSessionId)
    {
        var userId = Guid.Parse(Context.UserIdentifier!);
        var chatSession = await chatSessionRepository.GetChatSessionByIdAsync(chatSessionId);
        if (chatSession == null)
        {
            logger.LogWarning("User {UserId} attempted to leave non-existent chat session {ChatSessionId}", userId,
                chatSessionId);
            throw new HubException("Chat session does not exist");
        }

        EnsureUserIsParticipantInChat(userId, chatSession);

        await Groups.RemoveFromGroupAsync(Context.ConnectionId, chatSessionId.ToString());
        var key = GetInChatUserCacheKey(userId, chatSessionId);
        await cache.RemoveAsync(key);
    }

    [HubMethodName("Typing")]
    public async Task Typing(Guid chatSessionId)
    {
        var userId = Guid.Parse(Context.UserIdentifier!);
        var chatSession = await chatSessionRepository.GetChatSessionByIdAsync(chatSessionId);
        if (chatSession == null)
        {
            logger.LogWarning(
                "User {UserId} attempted to send typing notification to non-existent chat session {ChatSessionId}",
                userId, chatSessionId);
            throw new HubException("Chat session does not exist");
        }

        EnsureUserIsParticipantInChat(userId, chatSession);

        await Clients.Group(chatSessionId.ToString()).SendAsync("UserTyping", new
        {
            ChatSessionId = chatSessionId,
            SenderId = userId
        });
    }

    [HubMethodName("StopTyping")]
    public async Task StopTyping(Guid chatSessionId)
    {
        var userId = Guid.Parse(Context.UserIdentifier!);
        var chatSession = await chatSessionRepository.GetChatSessionByIdAsync(chatSessionId);
        if (chatSession == null)
        {
            logger.LogWarning(
                "User {UserId} attempted to send stop typing notification to non-existent chat session {ChatSessionId}",
                userId, chatSessionId);
            throw new HubException("Chat session does not exist");
        }

        chatSession.UserIsParticipantInChat(userId);

        await Clients.Group(chatSessionId.ToString()).SendAsync("UserStopTyping", new
        {
            ChatSessionId = chatSessionId,
            SenderId = userId
        });
    }

    [HubMethodName("GetChatHistory")]
    public async Task<IEnumerable<ChatMessage>> GetChatHistory(Guid chatSessionId)
    {
        var userId = Guid.Parse(Context.UserIdentifier!);
        var chatSession = await chatSessionRepository.GetChatSessionByIdAsync(chatSessionId);
        if (chatSession == null)
        {
            logger.LogWarning(
                "User {UserId} attempted to get chat history for non-existent chat session {ChatSessionId}", userId,
                chatSessionId);
            throw new HubException("Chat session does not exist");
        }

        EnsureUserIsParticipantInChat(userId, chatSession);

        var messages = await chatMessageRepository.GetMessagesByChatSessionIdAsync(chatSessionId);
        return messages;
    }

    public override async Task OnConnectedAsync()
    {
        var userId = Guid.Parse(Context.UserIdentifier!);

        var key = GetOnlineUserCacheKey(userId);
        await cache.SetStringAsync(key, Context.ConnectionId);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Guid.Parse(Context.UserIdentifier!);
        var chatSessions = await chatSessionRepository.GetChatSessionsByUserIdAsync(userId);
        foreach (var chatSession in chatSessions)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, chatSession.Id.ToString());
        }

        var onlineKey = GetOnlineUserCacheKey(userId);
        var inChatKey = GetInChatUserCacheKey(userId, Guid.Empty);
        await cache.RemoveAsync(onlineKey);
        await cache.RemoveAsync(inChatKey);
        await base.OnDisconnectedAsync(exception);
    }

    private void EnsureUserIsParticipantInChat(Guid userId, ChatSession chatSession)
    {
        if (chatSession.BuyerId != userId && chatSession.SellerId != userId) return;

        logger.LogWarning("User {UserId} is not a participant in chat session {ChatSessionId}", userId, chatSession.Id);
        throw new HubException("You are not a participant in this chat session");
    }

    private bool UserIsInChat(Guid userId, Guid chatSessionId)
    {
        var key = GetInChatUserCacheKey(userId, chatSessionId);
        var inChat = cache.GetString(key);
        return !string.IsNullOrEmpty(inChat);
    }

    private async Task<bool> IsUserOnlineAsync(Guid userId)
    {
        var key = GetOnlineUserCacheKey(userId);
        var connectionId = await cache.GetStringAsync(key);
        return !string.IsNullOrEmpty(connectionId);
    }

    private static string GetOnlineUserCacheKey(Guid userId) => $"{OnlineUserCacheKey}:{userId}";

    private static string GetInChatUserCacheKey(Guid userId, Guid chatSessionId) =>
        $"{InChatUserCacheKey}:{userId}:{chatSessionId}";
}