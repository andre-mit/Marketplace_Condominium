using Market.Domain.Enums;

namespace Market.SharedApplication.ViewModels.TransactionVIewModels;

public class ListTransactionViewModel
{
    public Guid Id { get; set; }
    
    public Guid SellerId { get; set; }
    public bool SellerConfirmed { get; set; }
    
    public Guid BuyerId { get; set; }
    public bool BuyerConfirmed { get; set; }
    
    public int ProductId { get; set; }
    public bool IsCompleted => SellerConfirmed && BuyerConfirmed;
    public DateTime? CompletedAt { get; set; }
    
    public required TransactionType TransactionType { get; set; }
    
    public ICollection<ListRatingViewModel> Ratings { get; set; } = new List<ListRatingViewModel>(2);
    
    public static implicit operator ListTransactionViewModel(Domain.Entities.Transaction transaction)
    {
        return new ListTransactionViewModel
        {
            Id = transaction.Id,
            SellerId = transaction.SellerId,
            SellerConfirmed = transaction.SellerConfirmed,
            BuyerId = transaction.BuyerId,
            BuyerConfirmed = transaction.BuyerConfirmed,
            ProductId = transaction.ProductId,
            CompletedAt = transaction.CompletedAt,
            TransactionType = transaction.TransactionType,
            Ratings = transaction.Ratings.Select(r => (ListRatingViewModel)r).ToList()
        };
    }
}