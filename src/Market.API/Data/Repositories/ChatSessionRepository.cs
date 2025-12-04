using Market.Domain.Entities;
using Market.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Market.API.Data.Repositories;

public class ChatSessionRepository(ApplicationDbContext context) : IChatSessionRepository
{
    public async Task<Guid> CreateChatSessionAsync(int productId, Guid customerId,
        CancellationToken cancellationToken = default)
    {
        var product = await context.Products.AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == productId, cancellationToken);
        if (product == null)
            throw new ArgumentException("Invalid product ID");

        if (product.OwnerId == customerId)
            throw new InvalidOperationException("Cannot create chat session with yourself");
        
        var existingChat = await context.ChatSessions.AsNoTracking()
            .FirstOrDefaultAsync(cs => cs.ProductId == productId &&
                                       cs.BuyerId == customerId &&
                                       cs.SellerId == product.OwnerId, cancellationToken);
        if (existingChat != null)
            return existingChat.Id;

        var chatSession = new ChatSession
        {
            ProductId = productId,
            BuyerId = customerId,
            SellerId = product.OwnerId
        };

        context.ChatSessions.Add(chatSession);
        await context.SaveChangesAsync(cancellationToken);
        return chatSession.Id;
    }

    public async Task<ChatSession?> GetChatSessionByIdAsync(Guid chatSessionId,
        CancellationToken cancellationToken = default)
    {
        return await context.ChatSessions
            .AsNoTracking()
            .Include(cs => cs.Messages)
            .Include(cs => cs.Buyer)
            .Include(cs => cs.Seller)
            .Include(cs => cs.Product)
            .ThenInclude(p => p.Images)
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

    public async Task<IEnumerable<ChatSession>> GetSyncChatSessionsAsync(Guid userId, DateTime lastSync,
        CancellationToken cancellationToken)
    {
        return await context.ChatSessions
            .AsNoTracking()
            .Where(cs => (cs.BuyerId == userId || cs.SellerId == userId) && cs.UpdatedAt > lastSync)
            .Include(cs => cs.Messages.Where(m => m.SentAt > lastSync))
            .Include(cs => cs.Buyer)
            .Include(cs => cs.Seller)
            .Include(cs => cs.Product)
            .ThenInclude(p => p.Images)
            .ToListAsync(cancellationToken);
    }
}