using Market.Domain.Enums;

namespace Market.SharedApplication.ViewModels.ProductViewModels;

/// <summary>
/// ViewModel for updating a product with generic image type
/// </summary>
/// <typeparam name="T">Type of the images (e.g., IFormFile[] for file uploads)</typeparam>
public class UpdateProductViewModel<T>
{
    public required string Name { get; set; }
    public required string Description { get; set; }
    public decimal? Price { get; set; }
    public string? ExchangeMessage { get; set; }
    
    public ProductCondition Condition { get; set; }

    public required TransactionType[] AdvertisementTypes { get; set; }
    
    public T? Images { get; set; }
    
    public string[]? ImagesToRemoveUrls { get; set; }
}