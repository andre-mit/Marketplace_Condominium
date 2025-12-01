using Market.Domain;
using Microsoft.EntityFrameworkCore;

namespace Market.API.Data.Repositories;

public class ProductRepository(ApplicationDbContext context) : IProductsRepository
{
    public async Task<PaginatedList<Product>> GetAllProductsAsync(int page, int pageSize,
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

        return new PaginatedList<Product>(products, total);
    }

    public async Task<PaginatedList<Product>> GetAvailableProductsAsync(int page, int pageSize,
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

        return new PaginatedList<Product>(products, total);
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

    public async Task<List<Product>> GetGroupedByCategoryProductsAsync(int limitByCategory,
        CancellationToken cancellationToken = default)
    {
        var query = context.Products
            .Include(p => p.Images)
            .Include(p => p.Category)
            .Include(p => p.Owner)
            .Where(p => p.IsAvailable)
            .GroupBy(p => p.CategoryId)
            .SelectMany(group =>
                group.OrderByDescending(p => p.CreatedAt)
                    .Take(limitByCategory));

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<List<Category>> GetAllCategoriesAsync(CancellationToken cancellationToken = default)
    {
        return await context.Categories.ToListAsync(cancellationToken);
    }

    public async Task<Product?> GetProductWithDetailsByIdAsync(int productId, CancellationToken cancellationToken = default)
    {
        return await context.Products
            .Include(p => p.Images)
            .Include(p => p.Owner)
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == productId, cancellationToken);
    }
}