using System.Security.Claims;
using Market.API.Helpers;
using Market.API.Services.Interfaces;
using Market.Domain.Repositories;
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

    [HttpPut("me")]
    public async Task<IActionResult> UpdateMe(UpdateUserPasswordViewModel model, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (model.Password != model.ConfirmPassword)
                return BadRequest("Passwords do not match");

            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (userId.IsNullOrWhiteSpace() || !Guid.TryParse(userId, out var userIdGuid))
                return Unauthorized("Invalid user ID");

            if (await userService.UpdateUserPasswordAsync(userIdGuid, model.Password, cancellationToken))
                return NoContent();
            
            return BadRequest("User not found");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating user password");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("{id:guid}/ratings")]
    public IActionResult GetUserRatings(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var ratings = transactionRepository.GetUserRatingsAsync(id, cancellationToken);
            return Ok(ratings);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving user ratings");
            return StatusCode(500, "Internal server error");
        }
    }
}