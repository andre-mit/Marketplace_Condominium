using Market.Domain.Entities;
using Market.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Market.API.Data.Repositories;

public class ProductRepository(ApplicationDbContext context) : IProductsRepository
{
    public async Task<(List<Product> products, int total)> GetAllProductsAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = context.Products.Where(p => p.IsAvailable).AsQueryable();
        var total = await query.CountAsync(cancellationToken);
        
        var products = await query
            .Include(p => p.Owner)
            .Include(p => p.Images)
            .Include(p => p.AdvertisementTypes)
            .OrderByDescending(p => p.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
        
        return (products, total);
    }

    public async Task<Product?> GetProductByIdAsync(int productId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task AddProductAsync(Product product, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task UpdateProductAsync(Product product, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task DeleteProductAsync(int productId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<int> GetTotalProductsCountAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}