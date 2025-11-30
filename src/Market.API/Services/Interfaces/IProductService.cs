using Market.SharedApplication.ViewModels.ProductViewModels;

namespace Market.API.Services.Interfaces;

public interface IProductService
{
    Task<List<ListCategorizedProducts>> ListCategorizedProductsAsync(int limitByCategory,
        CancellationToken cancellationToken = default);

    Task<int> CreateProductAsync(CreateProductViewModel<IFormFileCollection> createUpdateProductViewModel,
        Guid userId, CancellationToken cancellationToken = default);

    Task UpdateProductAsync(int productId, Guid userId,
        UpdateProductViewModel<IFormFileCollection> createUpdateProductViewModel,
        CancellationToken cancellationToken = default);
}