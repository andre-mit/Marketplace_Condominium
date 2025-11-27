using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using static BCrypt.Net.BCrypt;

namespace Market.API.Services;

public class AuthService(
    IConfiguration configuration,
    IUsersRepository usersRepository,
    IEmailService emailService,
    IUnitOfWork unitOfWork) : IAuthService
{
    public string CreateToken(User user)
    {
        var handler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(configuration.GetValue<string>("Jwt:PrivateKey")!);
        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(key),
            SecurityAlgorithms.HmacSha256Signature);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = GenerateClaims(user),
            Expires = DateTime.UtcNow.AddHours(2),
            SigningCredentials = credentials,
        };
        var token = handler.CreateToken(tokenDescriptor);
        return handler.WriteToken(token);
    }

    public User? Authenticate(string email, string password)
    {
        var user = usersRepository.GetByEmailOrCPF(email);
        if (user == null || !Verify(password, user.PasswordHash))
            return null;

        user.PasswordHash = null;

        return user;
    }

    public string HashPass(string password) => HashPassword(password);

    public async Task<bool> ForgotPasswordAsync(string email, CancellationToken cancellationToken = default)
    {
        var user = usersRepository.GetByEmailOrCPF(email);
        if (user == null)
            return false;

        var code = new Random().Next(100000, 999999).ToString();
        var added = usersRepository.AddResetPasswordCode(email, code);
        if (!added)
            return false;

        await unitOfWork.CommitAsync(cancellationToken);
        
        await emailService.SendPasswordResetEmailAsync(email, code, cancellationToken);

        return true;
    }
    
    public async Task<bool> ResetPasswordAsync(string email, string code, string newPassword,
        CancellationToken cancellationToken = default)
    {
        var newHashedPassword = HashPassword(newPassword);
        var success = usersRepository.ResetPassword(email, code, newHashedPassword);
        
        if (!success)
            return false;
        
        await unitOfWork.CommitAsync(cancellationToken);

        return true;
    }

    private static ClaimsIdentity GenerateClaims(User user)
    {
        var claims =
            new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.FirstName),
            });

        user.Roles?.ForEach(role => { claims.AddClaim(new Claim(ClaimTypes.Role, role.Name)); });

        return claims;
    }
}