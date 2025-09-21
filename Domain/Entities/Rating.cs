using Domain.Enums;

namespace Domain.Entities;

public class Rating
{
    public Guid Id { get; set; }
    public required Profile Rater { get; set; }
    public required Profile Ratee { get; set; }
    public RatingScore Score { get; set; }
}