using Market.Domain.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Market.API.Hubs;

[Authorize]
public class ChatHub(ILogger<ChatHub> logger, IChatSessionRepository chatSessionRepository, IChatMessageRepository chatMessageRepository) : Hub
{
    [HubMethodName("SendMessage")]
    public async Task SendMessage(Guid chatSessionId, string message)
    {
        var userId = Guid.Parse(Context.UserIdentifier!);
        var chatSession = await chatSessionRepository.GetChatSessionByIdAsync(chatSessionId);
        if (chatSession == null)
        {
            logger.LogWarning("User {UserId} attempted to send message to non-existent chat session {ChatSessionId}", userId, chatSessionId);
            throw new HubException("Chat session does not exist");
        }

        if (chatSession.BuyerId != userId && chatSession.SellerId != userId)
        {
            logger.LogWarning("User {UserId} attempted to send message to unauthorized chat session {ChatSessionId}", userId, chatSessionId);
            throw new HubException("You are not a participant in this chat session");
        }

        await chatMessageRepository.AddMessageAsync(chatSessionId, userId, message);
        await Clients.Group(chatSessionId.ToString()).SendAsync("ReceiveMessage", new
        {
            ChatSessionId = chatSessionId,
            SenderId = userId,
            Content = message,
            SentAt = DateTime.UtcNow
        });
    }
    
    [HubMethodName("Typing")]
    public async Task Typing(Guid chatSessionId)
    {
        var userId = Guid.Parse(Context.UserIdentifier!);
        var chatSession = await chatSessionRepository.GetChatSessionByIdAsync(chatSessionId);
        if (chatSession == null)
        {
            logger.LogWarning("User {UserId} attempted to send typing notification to non-existent chat session {ChatSessionId}", userId, chatSessionId);
            throw new HubException("Chat session does not exist");
        }

        if (chatSession.BuyerId != userId && chatSession.SellerId != userId)
        {
            logger.LogWarning("User {UserId} attempted to send typing notification to unauthorized chat session {ChatSessionId}", userId, chatSessionId);
            throw new HubException("You are not a participant in this chat session");
        }

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
            logger.LogWarning("User {UserId} attempted to send stop typing notification to non-existent chat session {ChatSessionId}", userId, chatSessionId);
            throw new HubException("Chat session does not exist");
        }

        if (chatSession.BuyerId != userId && chatSession.SellerId != userId)
        {
            logger.LogWarning("User {UserId} attempted to send stop typing notification to unauthorized chat session {ChatSessionId}", userId, chatSessionId);
            throw new HubException("You are not a participant in this chat session");
        }

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
            logger.LogWarning("User {UserId} attempted to get chat history for non-existent chat session {ChatSessionId}", userId, chatSessionId);
            throw new HubException("Chat session does not exist");
        }

        if (chatSession.BuyerId != userId && chatSession.SellerId != userId)
        {
            logger.LogWarning("User {UserId} attempted to get chat history for unauthorized chat session {ChatSessionId}", userId, chatSessionId);
            throw new HubException("You are not a participant in this chat session");
        }

        var messages = await chatMessageRepository.GetMessagesByChatSessionIdAsync(chatSessionId);
        return messages;
    }
    
    public override async Task OnConnectedAsync()
    {
        var userId = Guid.Parse(Context.UserIdentifier!);
        var chatSessions = await chatSessionRepository.GetChatSessionsByUserIdAsync(userId);
        foreach (var chatSession in chatSessions)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, chatSession.Id.ToString());
        }
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
        await base.OnDisconnectedAsync(exception);
    }
}