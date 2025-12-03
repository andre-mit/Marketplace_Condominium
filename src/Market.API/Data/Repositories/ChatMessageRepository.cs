using Market.Domain.Entities;
using Market.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Market.API.Data.Repositories;

public class ChatMessageRepository(ApplicationDbContext context) : IChatMessageRepository
{
    public async Task AddMessageAsync(Guid chatSessionId, Guid senderId, string message,
        CancellationToken cancellationToken = default)
    {
        var chatSession = await context.ChatSessions.Include(c => c.Messages)
            .FirstOrDefaultAsync(c => c.Id == chatSessionId, cancellationToken);
        if (chatSession == null)
            throw new ArgumentException("Invalid chat session ID");

        var chatMessage = new ChatMessage
        {
            ChatSessionId = chatSessionId,
            SenderId = senderId,
            Content = message,
            SentAt = DateTime.UtcNow
        };

        chatSession.UpdatedAt = DateTime.UtcNow;
        chatSession.Messages?.Add(chatMessage);

        await context.SaveChangesAsync(cancellationToken);
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