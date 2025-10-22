using Market.API.Data.Interfaces;
using Market.API.Services.Interfaces;
using Market.Domain.Entities;
using Market.Domain.Repositories;
using Market.SharedApplication.ViewModels.ProductViewModels;
using Microsoft.Extensions.Caching.Distributed;

namespace Market.API.Services;

public class ProductService(
    ILogger<ProductService> logger,
    IUnitOfWork unitOfWork,
    IProductsRepository productsRepository,
    IUploadFileService uploadFileService,
    IDistributedCache cache, IRedisKeyService redisKeyService) : IProductService
{
    public async Task<int> CreateProductAsync(CreateProductViewModel<IFormFileCollection> createProductViewModel,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var imageUrls = new List<string>();
        try
        {
            if (createProductViewModel.Images != null && createProductViewModel.Images.Count > 0)
            {
                foreach (var image in createProductViewModel.Images)
                {
                    await using var stream = image.OpenReadStream();
                    var imageUrl = await uploadFileService.UploadFileAsync(stream, image.FileName, "product-images",
                        cancellationToken);
                    imageUrls.Add(imageUrl);
                }
            }

            var product = new Product
            {
                Name = createProductViewModel.Name,
                Description = createProductViewModel.Description,
                Price = createProductViewModel.Price,
                Images = imageUrls.Select(url => new Image { Url = url }).ToList(),
                OwnerId = userId,
                Condition = createProductViewModel.Condition,
                AdvertisementTypes = createProductViewModel.AdvertisementTypes,
                ExchangeMessage = createProductViewModel.ExchangeMessage,
                IsAvailable = true
            };

            var productId = await productsRepository.AddProductAsync(product, cancellationToken);
            logger.LogInformation("Product {ProductId} created for user {UserId}", productId, userId);

            await unitOfWork.CommitAsync();

            return productId;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating product for user {UserId}", userId);
            // add images to cache for later deletion
            foreach (var imageUrl in imageUrls)
            {
                var cacheKey = $"orphaned-image-{Guid.NewGuid()}";
                await cache.SetStringAsync(cacheKey, imageUrl, new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1)
                }, cancellationToken);
            }
            
            throw;
        }
    }
    
    public async Task DeleteOrphanedImagesAsync(CancellationToken cancellationToken = default)
    {
        var keys = await redisKeyService.GetKeysByPrefixAsync("orphaned-image-", cancellationToken);
        foreach (var key in keys)
        {
            var imageUrl = await cache.GetStringAsync(key, cancellationToken);
            if (!string.IsNullOrEmpty(imageUrl))
            {
                try
                {
                    await uploadFileService.DeleteFileAsync(imageUrl, "", cancellationToken);
                    logger.LogInformation("Deleted orphaned image {ImageUrl}", imageUrl);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error deleting orphaned image {ImageUrl}", imageUrl);
                }
            }

            await cache.RemoveAsync(key, cancellationToken);
        }
    }
}