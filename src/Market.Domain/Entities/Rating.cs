using Market.Domain.Enums;

namespace Market.Domain.Entities;

public class Rating
{
    public Guid TransactionId { get; set; }
    public required Transaction Transaction { get; set; }
    
    public Guid RaterId { get; set; }
    public required User Rater { get; set; }
    
    public Guid RatedId { get; set; }
    public required User Rated { get; set; }
    public required string Review { get; set; }
    public RatingScore Score { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}