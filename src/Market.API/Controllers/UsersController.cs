using System.Security.Claims;
using Market.SharedApplication.ViewModels.UserViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Market.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController(
    ILogger<UsersController> logger,
    IUsersRepository usersRepository,
    ITransactionRepository transactionRepository,
    IUserService userService)
    : ControllerBase
{
    [HttpGet("me")]
    public IActionResult GetMe()
    {
        var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

        if (userId.IsNullOrWhiteSpace() || !Guid.TryParse(userId, out var guid))
            return Unauthorized("Invalid user ID");

        var user = usersRepository.GetById(guid);
        return Ok(user);
    }

    [HttpGet("{id:guid}/ratings")]
    public async Task<IActionResult> GetUserRatings(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var user = await userService.GetUserWithReviewsAsync(id, cancellationToken);
            if (user == null)
                return NotFound("User not found");

            return Ok(user);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving user ratings");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost("push-token")]
    public async Task<IActionResult> RegisterPushToken(RegisterPushTokenViewModel model,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (userId.IsNullOrWhiteSpace() || !Guid.TryParse(userId, out var userIdGuid))
                return Unauthorized("Invalid user ID");

            var response = await userService.RegisterPushTokenAsync(userIdGuid, model.Token, cancellationToken);

            if (!response.HasValue) return BadRequest("User not found");

            if (response.Value)
                return NoContent();

            return BadRequest("Failed to register push token");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error registering push token");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpDelete("push-token")]
    public async Task<IActionResult> UnregisterPushToken(CancellationToken cancellationToken = default)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (userId.IsNullOrWhiteSpace() || !Guid.TryParse(userId, out var userIdGuid))
                return Unauthorized("Invalid user ID");

            var response = await userService.UnregisterPushTokenAsync(userIdGuid, cancellationToken);

            if (!response.HasValue) return BadRequest("User not found");

            if (response.Value)
                return NoContent();

            return BadRequest("Failed to unregister push token");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error unregistering push token");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPut("me/password")]
    public async Task<IActionResult> UpdatePassword(UpdateUserPasswordViewModel model,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (userId.IsNullOrWhiteSpace() || !Guid.TryParse(userId, out var userIdGuid))
                return Unauthorized("Invalid user ID");

            var result = await userService.UpdateUserPasswordAsync(userIdGuid, model.CurrentPassword, model.Password,
                cancellationToken);

            if (result)
                return NoContent();

            return BadRequest("Failed to update password");
        }
        catch (UnauthorizedAccessException)
        {
            logger.LogWarning("Update password is incorrect");
            return Unauthorized("Current password is incorrect");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating user password");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPut("me/profile-picture")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UpdateProfilePicture([FromForm] IFormFile profilePicture,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (userId.IsNullOrWhiteSpace() || !Guid.TryParse(userId, out var userIdGuid))
                return Unauthorized("Invalid user ID");

            var result = await userService.UpdateUserProfilePictureAsync(userIdGuid, profilePicture, cancellationToken);
            if (result)
                return NoContent();

            return BadRequest("Failed to update profile picture");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating user profile picture");
            return StatusCode(500, "Internal server error");
        }
    }
}