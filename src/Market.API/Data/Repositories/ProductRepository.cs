using Market.Domain;
using Market.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Market.API.Data.Repositories;

public class ProductRepository(ApplicationDbContext context) : IProductsRepository
{
    public async Task<PaginatedList<Product>> GetProductsAsync(int pageNumber, int pageSize, string? searchTerm = null, int? categoryId = null,
        TransactionType? transactionType = null, ProductCondition? condition = null, bool? isAvailable = null, Guid? userId = null, CancellationToken cancellationToken = default)
    {
        var query = context.Products.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(p => p.Name.Contains(searchTerm) || p.Description.Contains(searchTerm));
        }

        if (categoryId.HasValue)
        {
            query = query.Where(p => p.CategoryId == categoryId.Value);
        }

        if (transactionType.HasValue)
        {
            query = query.Where(p => p.AdvertisementTypes.Contains(transactionType.Value));
        }

        if (condition.HasValue)
        {
            query = query.Where(p => p.Condition == condition.Value);
        }
        
        if (isAvailable.HasValue)
        {
            query = query.Where(p => p.IsAvailable == isAvailable.Value);
        }
        
        if(userId.HasValue)
        {
            query = query.Where(p => p.OwnerId == userId.Value);
        }

        var total = await query.CountAsync(cancellationToken);

        var products = await query
            .Include(p => p.Owner)
            .Include(p => p.Images)
            .OrderByDescending(p => p.CreatedAt)
            .ThenByDescending(p => p.UpdatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedList<Product>(products, total);
    }

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
        return await context.Products
            .Include(p => p.Images)
            .Include(p => p.Owner)
            .Include(p => p.Category)
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

    public async Task<List<CategoryWithProducts>> GetGroupedByCategoryProductsAsync(int limitByCategory,
        CancellationToken cancellationToken = default)
    {
        var products = await context.Products
            .Include(p => p.Images)
            .Include(p => p.Category)
            .Include(p => p.Owner)
            .Where(p => p.IsAvailable)
            .GroupBy(p => p.CategoryId)
            .Select(g => new CategoryWithProducts
            {
                Id = g.Key ?? 0,
                Name =  g.Key.HasValue ? g.First().Category.Name : "Outros",
                Icon = g.Key.HasValue ? g.First().Category.Icon : "dot-circle",
                Products = g.Take(limitByCategory).ToList()
            }).ToListAsync(cancellationToken);

        return products;
    }

    public async Task<List<Category>> GetAllCategoriesAsync(CancellationToken cancellationToken = default)
    {
        return await context.Categories.ToListAsync(cancellationToken);
    }

    public async Task<Product?> GetProductWithDetailsByIdAsync(int productId,
        CancellationToken cancellationToken = default)
    {
        return await context.Products
            .Include(p => p.Images)
            .Include(p => p.Owner)
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == productId, cancellationToken);
    }
}