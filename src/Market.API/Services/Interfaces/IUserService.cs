using Market.SharedApplication.ViewModels.UserViewModels;

namespace Market.API.Services.Interfaces;

public interface IUserService
{
    Task<ListUserViewModel> CreateUserAsync(CreateUserViewModel<IFormFile> model,
        CancellationToken cancellationToken = default);
    
    Task<bool> UpdateUserPasswordAsync(Guid userId, string newPassword, CancellationToken cancellationToken = default);
}