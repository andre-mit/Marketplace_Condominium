using Market.Domain.Entities;
using Market.Domain.Enums;

namespace Market.Domain.Repositories;

public interface ITransactionRepository
{
    Task<Transaction> CreateTransactionAsync(Transaction transaction, CancellationToken cancellationToken);

    Task ValidateTransactionAsync(Guid userId, Guid transactionId,
        CancellationToken cancellationToken = default);

    Task<Transaction?> GetTransactionAsync(Guid id, CancellationToken cancellationToken);

    Task<Rating> RateTransactionAsync(Guid transactionId, Guid raterId, RatingScore score, string review,
        CancellationToken cancellationToken);

    Task UpdateRateTransactionAsync(Guid transactionId, Guid raterId, RatingScore score, string review,
        CancellationToken cancellationToken);

    Task<List<Rating>> GetUserRatingsAsync(Guid userId, CancellationToken cancellationToken);
}