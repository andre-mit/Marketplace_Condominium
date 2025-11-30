using Market.Domain.Enums;

namespace Market.Domain.Entities;

public class Product
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public decimal? Price { get; set; }

    public string? ExchangeMessage { get; set; }

    public ProductCondition Condition { get; set; }

    public bool IsAvailable { get; set; } = true;

    public required Guid OwnerId { get; set; }
    public User? Owner { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public required TransactionType[] AdvertisementTypes { get; set; }

    public List<Image>? Images { get; set; }

    public Transaction? Transaction { get; set; }
    public ICollection<ChatSession>? ChatSessions { get; set; }
    
    public int? CategoryId { get; set; }
    public Category? Category { get; set; }
}