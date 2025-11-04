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
    IAuthService authService)
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
    public IActionResult UpdateMe(UpdateUserPasswordViewModel model)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (model.Password != model.ConfirmPassword)
                return BadRequest("Passwords do not match");

            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (userId.IsNullOrWhiteSpace() || !Guid.TryParse(userId, out var guid))
                return Unauthorized("Invalid user ID");

            var user = usersRepository.GetById(guid);
            if (user == null)
                return NotFound("User not found");

            user.PasswordHash = authService.HashPass(model.Password);

            usersRepository.Update(user);

            return NoContent();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating user password");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public IActionResult GetById(Guid id)
    {
        var user = usersRepository.GetById(id);
        if (user == null)
            return NotFound("User not found");

        return Ok(user);
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