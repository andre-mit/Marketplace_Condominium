using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Market.API.Services.Interfaces;
using Market.Domain.Entities;
using Market.Domain.Repositories;
using Microsoft.IdentityModel.Tokens;
using static BCrypt.Net.BCrypt;

namespace Market.API.Services;

public class AuthService(IConfiguration configuration, IUsersRepository usersRepository) : IAuthService
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
        var user = usersRepository.GetByEmail(email);
        if (user == null || !Verify(password, user.PasswordHash))
            return null;

        user.PasswordHash = null;
        
        return user;
    }
    
    public string HashPass(string password) => HashPassword(password);
    
    private static ClaimsIdentity GenerateClaims(User user)
    {
        var claims = 
        new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.FirstName),
        });
        
        user.Roles?.ForEach(role =>
        {
            claims.AddClaim(new Claim(ClaimTypes.Role, role.Name));
        });

        return claims;
    }
}