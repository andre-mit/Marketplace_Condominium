using Microsoft.AspNetCore.Mvc;

namespace Market.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    [HttpGet("me")]
    public IActionResult GetMe()
    {
        // This is just a placeholder implementation.
        // In a real application, you would retrieve the user information from the database or authentication context.
        var user = new
        {
            Id = Guid.NewGuid(),
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@test.com",
            Birth = DateOnly.FromDateTime(DateTime.Now.AddYears(-30)),
            Unit = "101",
            Tower = "A",
            CPF = "123.456.789-00",
            Roles = new List<string> { "Admin", "User" }
        };

        return Ok(user);
    }
}