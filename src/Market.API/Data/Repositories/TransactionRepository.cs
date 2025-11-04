using Market.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Market.API.Data.Repositories;

public class TransactionRepository(ApplicationDbContext context, IUnitOfWork unitOfWork) : ITransactionRepository
{
    public async Task<Transaction> CreateTransactionAsync(Transaction transaction, CancellationToken cancellationToken)
    {
        await context.Transactions.AddAsync(transaction, cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);

        return transaction;
    }

    public async Task<Transaction?> GetTransactionAsync(Guid id, CancellationToken cancellationToken)
    {
        return await context.Transactions
            .Include(t => t.Buyer)
            .Include(t => t.Seller)
            .Include(t => t.Product)
            .Include(t => t.Ratings)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    public async Task ValidateTransactionAsync(Guid userId, Guid transactionId,
        CancellationToken cancellationToken = default)
    {
        var transaction = await context.Transactions
            .FirstOrDefaultAsync(t => t.Id == transactionId, cancellationToken);
        
        if (transaction == null)
            throw new InvalidOperationException("Transaction not found.");
        
        var isBuyer = transaction.BuyerId == userId;
        
        if (isBuyer)
            transaction.BuyerConfirmed = true;
        else
            transaction.SellerConfirmed = true;
        
        context.Transactions.Update(transaction);
        await unitOfWork.CommitAsync(cancellationToken);
    }

    public async Task<Rating> RateTransactionAsync(Guid transactionId, Guid raterId, RatingScore score, string review,
        CancellationToken cancellationToken)
    {
        var transaction = await context.Transactions
            .Include(t => t.Buyer)
            .Include(t => t.Seller)
            .Include(t => t.Ratings)
            .FirstOrDefaultAsync(t => t.Id == transactionId, cancellationToken);

        if (transaction == null)
            throw new InvalidOperationException("Transaction not found.");
        
        if (transaction.Ratings.Any(r => r.RaterId == raterId))
            throw new InvalidOperationException("User has already rated this transaction.");

        var isSellerRater = transaction.Seller!.Id == raterId;
        var ratedId = isSellerRater ? transaction.Buyer!.Id : transaction.Seller.Id;

        var rating = new Rating
        {
            TransactionId = transactionId,
            RaterId = raterId,
            RatedId = ratedId,
            Score = score,
            Review = review,
            IsSellerRated = isSellerRater,
            CreatedAt = DateTime.UtcNow
        };

        await context.Ratings.AddAsync(rating, cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);

        return rating;
    }

    public async Task UpdateRateTransactionAsync(Guid transactionId, Guid raterId, RatingScore score, string review,
        CancellationToken cancellationToken)
    {
        var rating = await context.Ratings
            .FirstOrDefaultAsync(r => r.TransactionId == transactionId && r.RaterId == raterId, cancellationToken);

        if (rating == null)
            throw new InvalidOperationException("Rating not found.");

        rating.Score = score;
        rating.Review = review;
        rating.UpdatedAt = DateTime.UtcNow;

        context.Ratings.Update(rating);
        await unitOfWork.CommitAsync(cancellationToken);
    }
    
    public async Task<List<Rating>> GetUserRatingsAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await context.Ratings
            .Where(r => r.RatedId == userId)
            .ToListAsync(cancellationToken);
    }
}