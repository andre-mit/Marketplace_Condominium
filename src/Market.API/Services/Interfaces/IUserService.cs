using Market.SharedApplication.ViewModels.UserViewModels;

namespace Market.API.Services.Interfaces;

public interface IUserService
{
    Task<ListUserWithReviews?> GetUserWithReviewsAsync(Guid userId,
        CancellationToken cancellationToken = default);
    Task<ListUserViewModel> CreateUserAsync(CreateUserViewModel<IFormFile> model,
        CancellationToken cancellationToken = default);

    Task<bool> UpdateUserPasswordAsync(Guid userId, string currentPassword, string newPassword,
        CancellationToken cancellationToken = default);

    Task<bool?> RegisterPushTokenAsync(Guid userId, string token,
        CancellationToken cancellationToken = default);

    Task<bool?> UnregisterPushTokenAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<bool> UpdateUserProfilePictureAsync(Guid userId, IFormFile newImage,
        CancellationToken cancellationToken = default);
}