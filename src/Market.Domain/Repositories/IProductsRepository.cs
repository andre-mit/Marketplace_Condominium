using Market.Domain.Entities;
using Market.Domain.Enums;

namespace Market.Domain.Repositories;

public interface IProductsRepository
{
    Task<PaginatedList<Product>> GetProductsAsync(int pageNumber, int pageSize,
        string? searchTerm = null,
        int? categoryId = null,
        TransactionType? transactionType = null,
        ProductCondition? condition = null, bool? isAvailable = null, Guid? userId = null, CancellationToken cancellationToken = default);
    
    Task<PaginatedList<Product>> GetAllProductsAsync(int page, int pageSize, CancellationToken cancellationToken = default);
    Task<PaginatedList<Product>> GetAvailableProductsAsync(int page, int pageSize, CancellationToken cancellationToken = default);
    Task<Product?> GetProductByIdAsync(int productId, CancellationToken cancellationToken = default);
    Task<int> AddProductAsync(Product product, CancellationToken cancellationToken = default);
    void UpdateProduct(Product product, CancellationToken cancellationToken = default);
    Task DeleteProductAsync(int productId, CancellationToken cancellationToken = default);
    Task<int> GetTotalProductsCountAsync(CancellationToken cancellationToken = default);
    Task<List<CategoryWithProducts>> GetGroupedByCategoryProductsAsync(int limitByCategory, CancellationToken cancellationToken = default);
    Task<List<Category>> GetAllCategoriesAsync(CancellationToken cancellationToken = default);
    Task<Product?> GetProductWithDetailsByIdAsync(int productId, CancellationToken cancellationToken = default);
}