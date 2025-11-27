using System.Data;
using Market.API.Services.Interfaces;
using Market.Domain.Entities;
using Market.Domain.Repositories;
using Market.SharedApplication.ViewModels.AuthViewModels;
using Market.SharedApplication.ViewModels.UserViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Market.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(ILogger<AuthController> logger, IAuthService authService, IUserService userService)
    : ControllerBase
{
    [HttpPost("login")]
    public IActionResult GetToken([FromBody] LoginRequestViewModel model)
    {
        try
        {
            var user = authService.Authenticate(model.Identification, model.Password);
            if (user == null)
            {
                logger.LogWarning("Failed login attempt for email: {Email}", model.Identification);
                return Unauthorized("Invalid email or password");
            }

            var token = authService.CreateToken(user);

            return Ok(new LoginResponseViewModel
            {
                AccessToken = token,
                User = user
            });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("register")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> CreateUser(CreateUserViewModel<IFormFile> model)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await userService.CreateUserAsync(model);

            return Created("users/me", user);
        }
        catch (DuplicateNameException ex)
        {
            logger.LogWarning(ex, "Attempt to create a duplicate user with email: {Email} or CPF: {CPF}", model.Email,
                model.CPF);
            return Conflict("A user with the given email or CPF already exists.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating user");
            return BadRequest("An error occurred while creating the user.");
        }
    }
    
    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromQuery] string email, CancellationToken cancellationToken = default)
    {
        try
        {
            await authService.ForgotPasswordAsync(email, cancellationToken);
            return Ok("Password reset instructions have been sent to your email.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in forgot password for email: {Email}", email);
            return BadRequest("An error occurred while processing your request.");
        }
    }
    
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordViewModel model, CancellationToken cancellationToken = default)
    {
        try
        {
            var success = await authService.ResetPasswordAsync(model.Email, model.Code, model.Password, cancellationToken);
            if (!success)
                return BadRequest("Invalid reset code or email.");

            return Ok("Password has been reset successfully.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in reset password for email: {Email}", model.Email);
            return BadRequest("An error occurred while processing your request.");
        }
    }
}