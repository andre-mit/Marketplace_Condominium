using Market.Domain.Entities;

namespace Market.Domain.Repositories;

public interface IProductsRepository
{
    Task<PaginatedList<Product>> GetAllProductsAsync(int page, int pageSize, CancellationToken cancellationToken = default);
    Task<PaginatedList<Product>> GetAvailableProductsAsync(int page, int pageSize, CancellationToken cancellationToken = default);
    Task<Product?> GetProductByIdAsync(int productId, CancellationToken cancellationToken = default);
    Task<int> AddProductAsync(Product product, CancellationToken cancellationToken = default);
    void UpdateProduct(Product product, CancellationToken cancellationToken = default);
    Task DeleteProductAsync(int productId, CancellationToken cancellationToken = default);
    Task<int> GetTotalProductsCountAsync(CancellationToken cancellationToken = default);
    Task<List<Product>> GetGroupedByCategoryProductsAsync(int limitByCategory, CancellationToken cancellationToken = default);
}