using Market.Domain.Entities;
using Market.Domain.Enums;

namespace Market.SharedApplication.ViewModels.TransactionVIewModels;

public class ListRatingViewModel
{
    public Guid TransactionId { get; set; }
    
    public Guid RaterId { get; set; }
    
    public Guid RatedId { get; set; }
    
    public required string Review { get; set; }
    public RatingScore Score { get; set; }

    public static implicit operator ListRatingViewModel(Rating rating)
    {
        return new ListRatingViewModel
        {
            TransactionId = rating.TransactionId,
            RaterId = rating.RaterId,
            RatedId = rating.RatedId,
            Review = rating.Review,
            Score = rating.Score
        };
    }
}