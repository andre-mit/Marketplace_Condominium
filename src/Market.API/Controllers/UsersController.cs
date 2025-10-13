using System.Security.Claims;
using Market.API.Helpers;
using Market.API.Services.Interfaces;
using Market.Domain.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Market.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController(ILogger<UsersController> logger, IUsersRepository usersRepository, IAuthService authService)
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
    public IActionResult UpdateMe(UpdateUserViewModel model)
    {
        var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

        if (userId.IsNullOrWhiteSpace() || !Guid.TryParse(userId, out var guid))
            return Unauthorized("Invalid user ID");
        
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var user = usersRepository.GetById(guid);
        if (user == null)
            return NotFound("User not found");
        
        user.FirstName = model.FirstName;
        user.LastName = model.LastName;

        if (!model.Password.IsNullOrWhiteSpace())
            user.PasswordHash = authService.HashPass(model.Password);

        usersRepository.Update(user);
        return NoContent();
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
}