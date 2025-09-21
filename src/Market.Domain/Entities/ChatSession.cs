namespace Market.Domain.Entities;

public class ChatSession
{
    public Guid Id { get; set; }
    
    public Guid SellerId { get; set; }
    public required User Seller { get; set; }
    
    public Guid CustomerId { get; set; }
    public required User Customer { get; set; }
    
    public int ProductId { get; set; }
    public required Product Product { get; set; }
    
    public ICollection<ChatMessage>? Messages { get; set; } = new List<ChatMessage>();
    
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}