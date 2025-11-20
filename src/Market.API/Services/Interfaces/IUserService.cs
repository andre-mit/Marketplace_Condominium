namespace Market.API.Services.Interfaces;

public interface IUserService
{
    Task<bool> UpdateUserPasswordAsync(Guid userId, string newPassword, CancellationToken cancellationToken = default);
}