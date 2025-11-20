namespace Market.API.Services;

public class UserService(ILogger<UserService> logger, IUsersRepository usersRepository, IAuthService authService, IUnitOfWork unitOfWork) : IUserService
{
    public async Task<bool> UpdateUserPasswordAsync(Guid userId, string newPassword, CancellationToken cancellationToken = default)
    {
        var user = usersRepository.GetById(userId);
        if (user == null)
            return false;

        user.PasswordHash = authService.HashPass(newPassword);

        usersRepository.Update(user);
        await unitOfWork.CommitAsync(cancellationToken);
        
        logger.LogInformation("User updated successfully your password. UserId: {UserId}", userId);
        
        return true;
    }
}