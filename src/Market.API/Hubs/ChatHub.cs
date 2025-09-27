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

        if (chatSession.CustomerId != userId && chatSession.SellerId != userId)
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