using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Domain.Entities;
using Market.API.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace Market.API.Services;

public class TokenService(IConfiguration configuration) : ITokenService
{
    public string CreateToken(User user)
    {
        var handler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(configuration.GetValue<string>("Jwt:Key")!);
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
    
    private static ClaimsIdentity GenerateClaims(User user)
    {
        var claims = 
        new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.FirstName),
        });
        
        user.Roles.ForEach(role =>
        {
            claims.AddClaim(new Claim(ClaimTypes.Role, role.Name));
        });

        return claims;
    }
}