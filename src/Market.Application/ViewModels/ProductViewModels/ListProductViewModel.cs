using Market.Domain.Enums;
using Market.SharedApplication.ViewModels.CategoryViewModels;

namespace Market.SharedApplication.ViewModels.ProductViewModels;

public class ListProductViewModel
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public decimal? Price { get; set; }

    public required UserListForProductViewModel Owner { get; set; }
    
    public List<string>? ImageUrls { get; set; } = [];
    
    public ProductCondition Condition { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public required TransactionType[] AdvertisementTypes { get; set; }
    
    public ListCategoryViewModel? Category { get; set; }

    public class UserListForProductViewModel
    {
        public required string Name { get; set; }
        public string? ProfileImageUrl { get; set; }
        public byte Rating { get; set; }
    }
}