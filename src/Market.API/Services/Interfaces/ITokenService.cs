using Market.Domain.Entities;

namespace Market.API.Services.Interfaces;

public interface ITokenService
{
    string CreateToken(User user);
}