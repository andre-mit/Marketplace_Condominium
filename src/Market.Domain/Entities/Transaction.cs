using Market.Domain.Enums;

namespace Market.Domain.Entities;

public class Transaction
{
    public Guid Id { get; set; }
    
    public Guid SellerId { get; set; }
    public required User Seller { get; set; }
    public bool SellerConfirmed { get; set; } = false;
    
    public Guid BuyerId { get; set; }
    public required User Buyer { get; set; }
    public bool BuyerConfirmed { get; set; } = false;
    
    public int ProductId { get; set; }
    public required Product Product { get; set; }
    public bool IsCompleted => SellerConfirmed && BuyerConfirmed;
    public DateTime? CompletedAt { get; set; }
    
    public required TransactionType TransactionType { get; set; }
    
    public ICollection<Rating> Ratings { get; set; } = new List<Rating>(2);
}