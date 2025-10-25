using Market.Domain.Entities;
using Market.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Market.API.Data.Repositories;

public class ProductRepository(ApplicationDbContext context) : IProductsRepository
{
    public async Task<(List<Product> products, int total)> GetAllProductsAsync(int page, int pageSize,
        CancellationToken cancellationToken = default)
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

    public async Task<(List<Product> products, int total)> GetAvailableProductsAsync(int page, int pageSize,
        CancellationToken cancellationToken = default)
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
        return await context.Products.Include(p => p.Images)
            .Include(p => p.Owner)
            .FirstOrDefaultAsync(p => p.Id == productId, cancellationToken);
    }

    public async Task<int> AddProductAsync(Product product, CancellationToken cancellationToken = default)
    {
        await context.AddAsync(product, cancellationToken);
        return product.Id;
    }

    public void UpdateProduct(Product product, CancellationToken cancellationToken = default)
    {
        context.Update(product);
    }

    public async Task DeleteProductAsync(int productId, CancellationToken cancellationToken = default)
    {
        var product = await context.Products.FirstOrDefaultAsync(p => p.Id == productId, cancellationToken);
        if (product != null)
        {
            context.Products.Remove(product);
        }
    }

    public async Task<int> GetTotalProductsCountAsync(CancellationToken cancellationToken = default)
    {
        return await context.Products.CountAsync(cancellationToken);
    }
}