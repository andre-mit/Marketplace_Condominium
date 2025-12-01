using Market.SharedApplication.ViewModels.CategoryViewModels;
using Market.SharedApplication.ViewModels.ProductViewModels;

namespace Market.API.Services.Interfaces;

public interface IProductService
{
    Task<ListProductViewModel?> GetProductByIdAsync(int productId,
        CancellationToken cancellationToken = default);
    
    Task<List<ListCategorizedProductsViewModel>> ListCategorizedProductsAsync(int limitByCategory,
        CancellationToken cancellationToken = default);

    Task<int> CreateProductAsync(CreateProductViewModel<IFormFileCollection> createUpdateProductViewModel,
        Guid userId, CancellationToken cancellationToken = default);

    Task UpdateProductAsync(int productId, Guid userId,
        UpdateProductViewModel<IFormFileCollection> createUpdateProductViewModel,
        CancellationToken cancellationToken = default);

    Task<List<ListCategoryViewModel>> GetAllCategoriesAsync(CancellationToken cancellationToken = default);
}