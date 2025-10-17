using Market.Domain.Entities;

namespace Market.Domain.Repositories;

public interface IProductsRepository
{
    Task<(List<Product> products, int total)> GetAllProductsAsync(int page, int pageSize, CancellationToken cancellationToken = default);
    Task<Product?> GetProductByIdAsync(int productId, CancellationToken cancellationToken = default);
    Task<int> AddProductAsync(Product product, CancellationToken cancellationToken = default);
    Task UpdateProductAsync(Product product, CancellationToken cancellationToken = default);
    Task DeleteProductAsync(int productId, CancellationToken cancellationToken = default);
    Task<int> GetTotalProductsCountAsync(CancellationToken cancellationToken = default);
}