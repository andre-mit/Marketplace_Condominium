using System.Security.Claims;
using Market.API.Helpers;
using Market.API.Services.Interfaces;
using Market.Domain.Repositories;
using Market.SharedApplication.ViewModels.AuthViewModels;
using Market.SharedApplication.ViewModels.UserViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Market.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController(ILogger<UsersController> logger, IUsersRepository usersRepository, IAuthService authService)
    : ControllerBase
{
    [HttpPost]
    [AllowAnonymous]
    public IActionResult CreateUser(CreateUserViewModel model)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (usersRepository.UserAlreadyExists(model.Email, model.CPF))
                return Conflict("A user with the given email or CPF already exists.");

            var password = authService.HashPass(model.Password);
            var user = new Domain.Entities.User
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
            return CreatedAtAction(nameof(GetById), new { id = user.Id }, createdUser);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating user");
            return BadRequest("An error occurred while creating the user.");
        }
    }

    [HttpGet("me")]
    public IActionResult GetMe()
    {
        var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

        if (userId.IsNullOrWhiteSpace() || !Guid.TryParse(userId, out var guid))
            return Unauthorized("Invalid user ID");

        var user = usersRepository.GetById(guid);
        return Ok(user);
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
    
    [AllowAnonymous]
    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequestViewModel model)
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
            logger.LogError(ex, "Error during login for email: {Email}", model.Email);
            return BadRequest(ex.Message);
        }
    }
}