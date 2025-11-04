using Market.Domain.Enums;
using Market.SharedApplication.ViewModels.TransactionVIewModels;

namespace Market.API.Services;

public class TransactionService(ILogger<TransactionService> logger, ITransactionRepository transactionRepository, IProductsRepository productsRepository) : ITransactionService
{
    public async Task<ListTransactionViewModel> GetTransactionAsync(Guid transactionId, CancellationToken cancellationToken = default)
    {
        var transaction = await transactionRepository.GetTransactionAsync(transactionId, cancellationToken);
        if (transaction == null)
        {
            logger.LogWarning("Transaction with ID {TransactionId} not found.", transactionId);
            throw new ArgumentException("Transaction not found.");
        }
        
        logger.LogInformation("Transaction retrieved: {TransactionId}", transactionId);
        return transaction;
    }
    
    public async Task CreateTransactionAsync(Guid userId, TransactionType transactionType, int productId, CancellationToken cancellationToken = default)
    {
        var product = await productsRepository.GetProductByIdAsync(productId, cancellationToken);
        if (product == null)
        {
            logger.LogWarning("Product with ID {ProductId} not found.", productId);
            throw new ArgumentException("Product not found.");
        }
        
        var transaction = new Transaction
        {
            SellerId = product.OwnerId,
            BuyerId = userId,
            TransactionType = transactionType,
            ProductId = productId,
        };
        
        await transactionRepository.CreateTransactionAsync(transaction, cancellationToken);
        logger.LogInformation("Transaction created: {TransactionId} for ProductId: {ProductId}", transaction.Id, productId);
    }

    public async Task ValidateTransactionAsync(Guid userId, Guid transactionId, CancellationToken cancellationToken = default)
    {
        await transactionRepository.ValidateTransactionAsync(userId, transactionId, cancellationToken);
        logger.LogInformation("Transaction {TransactionId} validated by User {UserId}.", transactionId, userId);
    }

    public async Task RateTransactionAsync(Guid transactionId, Guid userId, string review, RatingScore score, CancellationToken cancellationToken = default)
    {
        await transactionRepository.RateTransactionAsync(transactionId, userId, score, review, cancellationToken);
        logger.LogInformation("Transaction {TransactionId} rated by User {UserId} with Score {Score}.", transactionId, userId, score);
    }
}