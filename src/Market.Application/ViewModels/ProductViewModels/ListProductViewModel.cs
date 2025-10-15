using Market.Domain.Enums;

namespace Market.SharedApplication.ViewModels.ProductViewModels;

public class ListProductViewModel
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public decimal? Price { get; set; }

    public required UserListForProductViewModel Owner { get; set; }
    
    public List<string> ImageUrls { get; set; } = [];

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public required TransactionType[] AdvertisementTypes { get; set; }

    public class UserListForProductViewModel
    {
        public required string Name { get; set; }
        public required string ProfileImageUrl { get; set; }
    }
}