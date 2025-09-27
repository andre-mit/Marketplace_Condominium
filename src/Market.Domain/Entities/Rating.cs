using Domain.Enums;

namespace Market.Domain.Entities;

public class Rating
{
    public Guid Id { get; set; }
    public required string Review { get; set; }
    public RatingScore Score { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}