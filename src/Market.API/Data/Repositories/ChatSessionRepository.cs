using Market.Domain.Entities;
using Market.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Market.API.Data.Repositories;

public class ChatSessionRepository(ApplicationDbContext context) : IChatSessionRepository
{
    public async Task<Guid> CreateChatSessionAsync(int productId, Guid customerId)
    {
        var product = await context.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == productId);
        if (product == null)
            throw new ArgumentException("Invalid product ID");
        
        if (product.OwnerId == customerId)
            throw new InvalidOperationException("Cannot create chat session with yourself");
        
        var chatSession = new ChatSession
        {
            ProductId = productId,
            BuyerId = customerId,
            SellerId = product.OwnerId
        };
        
        context.ChatSessions.Add(chatSession);
        await context.SaveChangesAsync();
        return chatSession.Id;
    }

    public async Task<ChatSession?> GetChatSessionByIdAsync(Guid chatSessionId)
    {
        return await context.ChatSessions
            .AsNoTracking()
            .Include(cs => cs.Messages)
            .FirstOrDefaultAsync(cs => cs.Id == chatSessionId);
    }

    public async Task<IEnumerable<ChatSession>> GetChatSessionsByUserIdAsync(Guid userId)
    {
        return await context.ChatSessions
            .AsNoTracking()
            .Where(cs => cs.BuyerId == userId || cs.SellerId == userId)
            .Include(cs => cs.Messages)
            .ToListAsync();
    }
}