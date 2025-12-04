using Market.Domain.Entities;

namespace Market.SharedApplication.ViewModels.UserViewModels;

public class ListUserWithReviews
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; }
    public required string Phone { get; set; }
    public string? AvatarUrl { get; set; }
    public decimal Rating { get; set; }
    public uint RatingsCount { get; set; }
    public DateOnly CreatedAt { get; set; }
    
    public required List<ReviewViewModel> Reviews { get; set; }

    public static implicit operator ListUserWithReviews(User user) => new ListUserWithReviews
    {
        Id = user.Id,
        Name = $"{user.FirstName} {user.LastName}",
        Email = user.Email,
        Phone = user.Phone,
        AvatarUrl = user.AvatarUrl,
        Rating = (decimal)user.Rating / user.RatingsCount,
        RatingsCount = user.RatingsCount,
        CreatedAt = DateOnly.FromDateTime(user.CreatedAt),
        Reviews = user.ReceivedRatings?.Where(r => r.IsSellerRated).Select(r => new ReviewViewModel
        {
            Id = r.TransactionId,
            ReviewerName = $"{r.Rater?.FirstName} {r.Rater?.LastName}",
            ReviewerAvatarUrl = r.Rater?.AvatarUrl ?? string.Empty,
            Comment = r.Review,
            Rating = (uint)r.Score,
            CreatedAt = DateOnly.FromDateTime(r.CreatedAt)
        }).ToList() ?? []
    };
}
public class ReviewViewModel
{
    public Guid Id { get; set; }
    public required string ReviewerName { get; set; }
    public required string ReviewerAvatarUrl { get; set; }
    public required string Comment { get; set; }
    public uint Rating { get; set; }
    public DateOnly CreatedAt { get; set; }
}