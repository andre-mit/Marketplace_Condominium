using Domain.Enums;

namespace Market.Application.ViewModels.ProductViewModels;

public class ListProductViewModel
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required string? ImageUrl { get; set; }
    public decimal? Price { get; set; }

    public required UserListForProductViewModel Owner { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public required ProductAdvertisementType[] AdvertisementTypes { get; set; }

    public class UserListForProductViewModel
    {
        public required string Name { get; set; }
        public required string ProfileImageUrl { get; set; }
    }
}