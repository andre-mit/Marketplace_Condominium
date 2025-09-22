using Domain.Enums;

namespace Market.Domain.Entities;

public class Product
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required string? ImageUrl { get; set; }
    public decimal? Price { get; set; }
    public string? ExchangeMessage { get; set; }
    public bool IsAvailable { get; set; } = true;
    
    public Guid OwnerId { get; set; }
    public required User Owner { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? UpdatedAt { get; set; }

    public required ProductAdvertisementType[] AdvertisementTypes { get; set; }
    
    public ICollection<Image>? Images { get; set; } = new List<Image>();
    
    public ICollection<Sale>? Sales { get; set; } = new List<Sale>();
    public ICollection<ChatSession>? ChatSessions { get; set; }
}