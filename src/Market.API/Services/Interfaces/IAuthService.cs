namespace Market.API.Services.Interfaces;

public interface IAuthService
{
    string CreateToken(User user);
    User? Authenticate(string email, string password);
    string HashPass(string password);
    Task<bool> ForgotPasswordAsync(string email, CancellationToken cancellationToken);

    Task<bool> ResetPasswordAsync(string email, string code, string newPassword,
        CancellationToken cancellationToken = default);
}