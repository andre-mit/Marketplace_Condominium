using Market.SharedApplication.ViewModels.ProductViewModels;

namespace Market.API.Services.Interfaces;

public interface IProductService
{
    Task<int> CreateProductAsync(CreateProductViewModel<IFormFileCollection> createProductViewModel,
        Guid userId, CancellationToken cancellationToken = default);
}