using Market.Domain.Entities;
using Market.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Market.API.Data.Repositories;

public class ChatMessageRepository(ApplicationDbContext context) : IChatMessageRepository
{
    public async Task AddMessageAsync(Guid chatSessionId, Guid senderId, string message)
    {
        var chatSession = await context.ChatSessions.FindAsync(chatSessionId);
        if (chatSession == null)
            throw new ArgumentException("Invalid chat session ID");
        
        var chatMessage = new ChatMessage
        {
            ChatSessionId = chatSessionId,
            SenderId = senderId,
            Content = message,
            SentAt = DateTime.UtcNow
        };
        
        context.ChatMessages.Add(chatMessage);
        await context.SaveChangesAsync();
    }

    public async Task<IEnumerable<ChatMessage>> GetMessagesByChatSessionIdAsync(Guid chatSessionId)
    {
        return await context.ChatMessages
            .AsNoTracking()
            .Where(cm => cm.ChatSessionId == chatSessionId)
            .OrderBy(cm => cm.SentAt)
            .ToListAsync();
    }
}