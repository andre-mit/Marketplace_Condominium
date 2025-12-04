using Market.Domain;
using Market.Domain.Enums;
using Market.SharedApplication.ViewModels.CategoryViewModels;
using Market.SharedApplication.ViewModels.ProductViewModels;

namespace Market.API.Services.Interfaces;

public interface IProductService
{
    Task<PaginatedList<ListProductViewModel>> ListProductsAsync(int pageNumber, int pageSize,
        string? searchTerm = null,
        int? categoryId = null,
        TransactionType? transactionType = null,
        ProductCondition? condition = null,
        CancellationToken cancellationToken = default);

    Task<PaginatedList<ListProductViewModel>> ListMineProductsAsync(
        Guid userId,
        int pageNumber,
        int pageSize,
        string? searchTerm = null,
        int? categoryId = null,
        TransactionType? transactionType = null,
        ProductCondition? condition = null,
        bool? isAvailable = null,
        CancellationToken cancellationToken = default);
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