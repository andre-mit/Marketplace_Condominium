using Market.SharedApplication.ViewModels.CategoryViewModels;
using Market.SharedApplication.ViewModels.ProductViewModels;
using Microsoft.Extensions.Caching.Distributed;

namespace Market.API.Services;

public class ProductService(
    ILogger<ProductService> logger,
    IUnitOfWork unitOfWork,
    IProductsRepository productsRepository,
    IUploadFileService uploadFileService,
    IDistributedCache cache) : IProductService
{
    public async Task<ListProductViewModel?> GetProductByIdAsync(int productId,
        CancellationToken cancellationToken = default)
    {
        var product = await productsRepository.GetProductByIdAsync(productId, cancellationToken);
        
        if (product != null) return (ListProductViewModel)product;
        
        logger.LogInformation("Product {ProductId} not found", productId);
        return null;

    }

    public async Task<List<ListCategorizedProductsViewModel>> ListCategorizedProductsAsync(int limitByCategory,
        CancellationToken cancellationToken = default)
    {
        var response = await productsRepository.GetGroupedByCategoryProductsAsync(limitByCategory, cancellationToken);

        var categorizedProducts = response.Select(c => new ListCategorizedProductsViewModel
        {
            Id = c.Id,
            Name = c.Name,
            Icon = c.Icon!,
            Products = c.Products!.Select(prod => (ListProductViewModel)prod).ToList()
        });

        return categorizedProducts.ToList();
    }

    public async Task<int> CreateProductAsync(CreateProductViewModel<IFormFileCollection> createUpdateProductViewModel,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var imageUrls = new List<string>();
        try
        {
            if (createUpdateProductViewModel.Images is { Count: > 0 })
            {
                foreach (var image in createUpdateProductViewModel.Images)
                {
                    await using var stream = image.OpenReadStream();
                    var imageUrl = await uploadFileService.UploadFileAsync(stream, image.FileName,
                        "",
                        image.ContentType,
                        Constants.ProductImagesBucket,
                        cancellationToken);
                    imageUrls.Add(imageUrl);
                }
            }

            var product = new Product
            {
                Name = createUpdateProductViewModel.Name,
                Description = createUpdateProductViewModel.Description,
                Price = createUpdateProductViewModel.Price,
                Images = imageUrls.Select(url => new Image { Url = url }).ToList(),
                OwnerId = userId,
                Condition = createUpdateProductViewModel.Condition,
                AdvertisementTypes = createUpdateProductViewModel.AdvertisementTypes,
                ExchangeMessage = createUpdateProductViewModel.ExchangeMessage,
                IsAvailable = true,
                CategoryId = createUpdateProductViewModel.CategoryId,
                CreatedAt = DateTime.UtcNow,
            };

            var productId = await productsRepository.AddProductAsync(product, cancellationToken);
            logger.LogInformation("Product {ProductId} created for user {UserId}", productId, userId);

            await unitOfWork.CommitAsync(cancellationToken);

            return productId;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating product for user {UserId}", userId);
            // add images to cache for later deletion
            foreach (var imageUrl in imageUrls)
            {
                var cacheKey = $"{Constants.OrphanedImagePrefix}_{Guid.NewGuid()}";
                await cache.SetStringAsync(cacheKey, imageUrl, new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1)
                }, cancellationToken);
            }

            throw;
        }
    }

    public async Task UpdateProductAsync(int productId, Guid userId,
        UpdateProductViewModel<IFormFileCollection> createUpdateProductViewModel,
        CancellationToken cancellationToken = default)
    {
        var product = await productsRepository.GetProductByIdAsync(productId, cancellationToken);
        if (product == null)
        {
            throw new KeyNotFoundException($"Product with ID {productId} not found");
        }

        if (product.OwnerId != userId)
        {
            throw new UnauthorizedAccessException("User is not the owner of the product");
        }

        product.Name = createUpdateProductViewModel.Name;
        product.Description = createUpdateProductViewModel.Description;
        product.Price = createUpdateProductViewModel.Price;
        product.Condition = createUpdateProductViewModel.Condition;
        product.AdvertisementTypes = createUpdateProductViewModel.AdvertisementTypes;
        product.ExchangeMessage = createUpdateProductViewModel.ExchangeMessage;
        product.UpdatedAt = DateTime.UtcNow;
        if (createUpdateProductViewModel.ImagesToRemoveUrls != null)
            product.Images?.RemoveAll(img => createUpdateProductViewModel.ImagesToRemoveUrls.Contains(img.Url));

        if (createUpdateProductViewModel.Images is { Count: > 0 })
        {
            foreach (var image in createUpdateProductViewModel.Images)
            {
                await using var stream = image.OpenReadStream();
                var imageUrl = await uploadFileService.UploadFileAsync(stream, image.FileName,
                    "",
                    image.ContentType,
                    Constants.ProductImagesBucket,
                    cancellationToken);
                product.Images?.Add(new Image { Url = imageUrl });
            }
        }

        productsRepository.UpdateProduct(product, cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);

        foreach (var imageUrl in createUpdateProductViewModel.ImagesToRemoveUrls ?? [])
        {
            var cacheKey = $"{Constants.OrphanedImagePrefix}_{Guid.NewGuid()}";
            await cache.SetStringAsync(cacheKey, imageUrl, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1)
            }, cancellationToken);
        }

        logger.LogInformation("Product {ProductId} updated", productId);
    }

    public async Task<List<ListCategoryViewModel>> GetAllCategoriesAsync(CancellationToken cancellationToken = default)
    {
        var categories = await productsRepository.GetAllCategoriesAsync(cancellationToken);
        logger.LogInformation("Fetched {CategoryCount} categories", categories.Count);
        return categories.Select(c => new ListCategoryViewModel
        {
            Id = c.Id,
            Name = c.Name,
            Icon = c.Icon
        }).ToList();
    }
}