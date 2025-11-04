using Market.Domain.Enums;
using Market.SharedApplication.ViewModels.TransactionVIewModels;

namespace Market.API.Services.Interfaces;

public interface ITransactionService
{
    Task<ListTransactionViewModel> GetTransactionAsync(Guid transactionId, CancellationToken cancellationToken = default);
    Task CreateTransactionAsync(Guid userId, TransactionType transactionType, int productId, CancellationToken cancellationToken = default);
    
    Task ValidateTransactionAsync(Guid userId, Guid transactionId, CancellationToken cancellationToken = default);
    
    Task RateTransactionAsync(Guid transactionId, Guid userId, string review, RatingScore score, CancellationToken cancellationToken = default);
}