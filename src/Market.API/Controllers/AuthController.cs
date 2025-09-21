using Market.API.Services.Interfaces;
using Market.Application.ViewModels.AuthViewModels;
using Market.Domain.Entities;
using Market.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Market.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(ILogger<AuthController> logger, IAuthService authService) : ControllerBase
{
    [HttpPost("token")]
    public IActionResult GetToken([FromBody] LoginRequestViewModel model)
    {
        try
        {
            var user = authService.Authenticate(model.Email, model.Password);
            if (user == null)
            {
                logger.LogWarning("Failed login attempt for email: {Email}", model.Email);
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
}