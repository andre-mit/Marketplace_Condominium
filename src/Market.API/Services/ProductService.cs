using Market.API.Services.Interfaces;
using Market.Domain.Entities;
using Market.Domain.Repositories;
using Market.SharedApplication.ViewModels.ProductViewModels;

namespace Market.API.Services;

public class ProductService(ILogger<ProductService> logger, IProductsRepository productsRepository, IUploadFileService uploadFileService) : IProductService
{
    public async Task<int> CreateProductAsync(CreateProductViewModel<IFormFileCollection> createProductViewModel, Guid userId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var imageUrls = new List<string>();
            if (createProductViewModel.Images != null && createProductViewModel.Images.Count > 0)
            {
                foreach (var image in createProductViewModel.Images)
                {
                    using var stream = image.OpenReadStream();
                    var imageUrl = await uploadFileService.UploadFileAsync(stream, image.FileName, "product-images", cancellationToken);
                    imageUrls.Add(imageUrl);
                }
            }
            
            var product = new Product
            {
                Name = createProductViewModel.Name,
                Description = createProductViewModel.Description,
                Price = createProductViewModel.Price,
                Images = imageUrls.Select(url => new Image { Url = url}).ToList(),
                OwnerId = userId,
                Condition = createProductViewModel.Condition,
                AdvertisementTypes = createProductViewModel.AdvertisementTypes,
                ExchangeMessage = createProductViewModel.ExchangeMessage,
                IsAvailable = true
            };

            var productId = await productsRepository.AddProductAsync(product, cancellationToken);
            logger.LogInformation("Product {ProductId} created for user {UserId}", productId, userId);
            
            return productId;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating product for user {UserId}", userId);
            throw;
        }
    }
}