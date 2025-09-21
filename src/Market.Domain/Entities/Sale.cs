namespace Domain.Entities;

public class Sale
{
    public Guid Id { get; set; }
    public required User Seller { get; set; }
    public bool SellerConfirmed { get; set; } = false;
    public required User Buyer { get; set; }
    public bool BuyerConfirmed { get; set; } = false;
    public required Product Product { get; set; }
    public DateTime SaleDate { get; set; } = DateTime.Now;
    public bool IsCompleted => SellerConfirmed && BuyerConfirmed;
    public DateTime? CompletedAt { get; set; }
    
    public Rating? SellerRating { get; set; }
    public Rating? BuyerRating { get; set; }
}