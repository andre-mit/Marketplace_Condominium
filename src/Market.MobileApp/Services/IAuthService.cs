using Market.SharedApplication.ViewModels.UserViewModels;

namespace Market.MobileApp.Services
{
    public interface IAuthService
    {
        Task LoginAsync(string email, string password);
        void Logout();
        Task<string?> GetAuthTokenAsync();
        Task<bool> IsUserAuthenticatedAsync();
        Task RegisterAsync(CreateUserViewModel user);
        Task ChangePasswordAsync(string currentPassword, string newPassword);
        Task SendPasswordResetEmailAsync(string email);
        Task<bool> ConfirmEmailAsync(string userId, string token);

    }
}
