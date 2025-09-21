using Market.Domain.Entities;

namespace Market.API.Services.Interfaces;

public interface IAuthService
{
    string CreateToken(User user);
    User? Authenticate(string email, string password);
    string HashPass(string password);
}