using Domain.Enums;

namespace Domain.Entities;

public class Rating
{
    public Guid Id { get; set; }
    public required User Rater { get; set; }
    public required User Ratee { get; set; }
    public RatingScore Score { get; set; }
}