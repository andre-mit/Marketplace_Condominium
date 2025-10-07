namespace Market.Domain.Entities;

public class ChatSession
{
    public Guid Id { get; set; }
    
    public Guid SellerId { get; set; }
    public User Seller { get; set; }
    
    public Guid BuyerId { get; set; }
    public User? Buyer { get; set; }
    
    public int ProductId { get; set; }
    public Product? Product { get; set; }
    
    public ICollection<ChatMessage>? Messages { get; set; } = new List<ChatMessage>();
    
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}