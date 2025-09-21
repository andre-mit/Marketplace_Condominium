using Domain.Entities;
using Market.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Market.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(ITokenService tokenService) : ControllerBase
{
    [HttpPost("token")]
    public IActionResult GetToken()
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@test.com",
            Birth = default,
            Unit = "101",
            Tower = "A",

        };
        var token = tokenService.CreateToken(user);
        return Ok(new { token });
    }
}