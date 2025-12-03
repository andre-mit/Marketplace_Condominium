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
        public decimal Rating { get; set; }
        public int RatingsCount { get; set; }
    }

    public static implicit operator ListProductViewModel(Domain.Entities.Product product) => new ListProductViewModel
    {
        Id = product.Id,
        Name = product.Name,
        Description = product.Description,
        Price = product.Price,
        Owner =  new UserListForProductViewModel
        {
            Name = $"{product.Owner!.FirstName} {product.Owner.LastName}",
            ProfileImageUrl = product.Owner.AvatarUrl,
            Rating = (decimal)product.Owner.Rating / product.Owner.RatingsCount,
            RatingsCount = product.Owner.RatingsCount
        },
        ImageUrls = product.Images?.Select(i => i.Url).ToList(),
        Condition = product.Condition,
        CreatedAt = product.CreatedAt,
        UpdatedAt = product.UpdatedAt,
        AdvertisementTypes = product.AdvertisementTypes,
        Category = product.Category != null ? new ListCategoryViewModel
        {
            Id = product.Category.Id,
            Name = product.Category.Name,
            Icon = product.Category.Icon
        } : null
    };
}