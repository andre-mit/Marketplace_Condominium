using System.Data;
using Market.SharedApplication.ViewModels.UserViewModels;
using Microsoft.Extensions.Caching.Distributed;

namespace Market.API.Services;

public class UserService(
    ILogger<UserService> logger,
    IUsersRepository usersRepository,
    IAuthService authService,
    IUploadFileService uploadFileService,
    IDistributedCache cache,
    IUnitOfWork unitOfWork) : IUserService
{
    public async Task<ListUserViewModel> CreateUserAsync(CreateUserViewModel<IFormFile> model,
        CancellationToken cancellationToken = default)
    {
        if (usersRepository.UserAlreadyExists(model.Email, model.CPF))
            throw new DuplicateNameException("A user with the given email or CPF already exists.");

        string? imageUrl = null;

        try
        {
            if (model.Image != null)
            {
                await using var stream = model.Image.OpenReadStream();
                imageUrl = await uploadFileService.UploadFileAsync(stream, model.Image.FileName,
                    "",
                    model.Image.ContentType,
                    Constants.UserAvatarsBucket,
                    cancellationToken);
            }

            var password = authService.HashPass(model.Password);
            var user = new User
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Cpf = model.CPF,
                Email = model.Email,
                PasswordHash = password,
                Birth = model.Birth,
                Unit = model.Unit,
                Tower = model.Tower,
                Phone = model.Phone,
                AvatarUrl = imageUrl
            };

            usersRepository.Add(user);
            await unitOfWork.CommitAsync(cancellationToken);

            logger.LogInformation("User created successfully. UserId: {UserId}", user.Id);

            return user;
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message, "Error creating user with email: {Email}", model.Email);

            if (imageUrl == null) throw;

            var cacheKey = $"{Constants.OrphanedImagePrefix}_{Guid.NewGuid()}";
            await cache.SetStringAsync(cacheKey, imageUrl, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1)
            }, cancellationToken);

            throw;
        }
    }

    public async Task<bool> UpdateUserPasswordAsync(Guid userId, string newPassword,
        CancellationToken cancellationToken = default)
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

    public async Task<bool?> RegisterPushTokenAsync(Guid userId, string token,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await usersRepository.UpdateNotificationTokenAsync(userId, token, cancellationToken);
            await unitOfWork.CommitAsync(cancellationToken);
            
            logger.LogInformation("Push token registered successfully for user {UserId}", userId);
            
            return true;
        }
        catch (KeyNotFoundException x)
        {
            logger.LogWarning(x, "User not found when registering push token. UserId: {UserId}", userId);
            return null;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error registering push token for user {UserId}", userId);
            return false;
        }
    }

    public async Task<bool?> UnregisterPushTokenAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            await usersRepository.UpdateNotificationTokenAsync(userId, null, cancellationToken);
            await unitOfWork.CommitAsync(cancellationToken);
            
            logger.LogInformation("Push token unregistered successfully for user {UserId}", userId);

            return true;
        }
        catch (KeyNotFoundException x)
        {
            logger.LogWarning(x, "User not found when unregistering push token. UserId: {UserId}", userId);
            return null;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error unregistering push token for user {UserId}", userId);
            return false;
        }
    }
}