using Market.SharedApplication.ViewModels.ProductViewModels;

namespace Market.API.Services.Interfaces;

public interface IProductService
{
    Task<int> CreateProductAsync(CreateProductViewModel createProductViewModel, IFormFileCollection? images,
        Guid userId, CancellationToken cancellationToken = default);
}