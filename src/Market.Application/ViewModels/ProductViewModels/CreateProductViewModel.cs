using Market.Domain.Enums;

namespace Market.SharedApplication.ViewModels.ProductViewModels;

public class CreateProductViewModel
{
    public required string Name { get; set; }
    public required string Description { get; set; }
    public decimal? Price { get; set; }
    public string? ExchangeMessage { get; set; }
    
    public ProductCondition Condition { get; set; }

    public required TransactionType[] AdvertisementTypes { get; set; }
    
    public virtual List<object> Images { get; set; }
}