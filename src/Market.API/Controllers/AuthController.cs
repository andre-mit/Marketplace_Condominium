using Market.API.Services.Interfaces;
using Market.Domain.Entities;
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
            CPF = "123.456.789-00",
            Roles = new List<Role>
            {
                new() { Id = Guid.NewGuid(), Name = "Admin" },
                new() { Id = Guid.NewGuid(), Name = "User" }
            }
        };
        var token = tokenService.CreateToken(user);
        return Ok(new { token });
    }
}