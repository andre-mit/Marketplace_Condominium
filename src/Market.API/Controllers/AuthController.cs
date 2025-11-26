using Market.API.Services.Interfaces;
using Market.Domain.Entities;
using Market.Domain.Repositories;
using Market.SharedApplication.ViewModels.AuthViewModels;
using Market.SharedApplication.ViewModels.UserViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Market.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(ILogger<AuthController> logger, IUsersRepository usersRepository, IAuthService authService) : ControllerBase
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
    public IActionResult CreateUser(CreateUserViewModel model)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (usersRepository.UserAlreadyExists(model.Email, model.CPF))
                return Conflict("A user with the given email or CPF already exists.");

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
                Tower = model.Tower
            };

            usersRepository.Add(user);

            ListUserViewModel createdUser = user;
            return Created("users/me", createdUser);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating user");
            return BadRequest("An error occurred while creating the user.");
        }
    }
}