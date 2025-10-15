using Market.API.Services.Interfaces;
using Market.SharedApplication.ViewModels.ProductViewModels;

namespace Market.API.Services;

public class ProductService : IProductService
{
    public Task<int> CreateProductAsync(CreateProductViewModel createProductViewModel, IFormFileCollection? images, Guid userId,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}