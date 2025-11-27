using Microsoft.AspNetCore.Mvc;
using UsersApi.Models;

namespace UsersApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> ListUsers()
    {
        try
        {
            var usersData = await System.IO.File.ReadAllTextAsync("users.json");
            var users = System.Text.Json.JsonSerializer.Deserialize<List<User>>(usersData)!;
            return Ok(users);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Error = ex.Message });
        }
    }

    [HttpPost("verify")]
    public async Task<IActionResult> VerifyUserAsync([FromBody] User model)
    {
        try
        {
            model.Normalize();
            var usersData = await System.IO.File.ReadAllTextAsync("users.json");
            var users = System.Text.Json.JsonSerializer.Deserialize<List<User>>(usersData)!;

            users.ForEach(user => user.Normalize());

            var userExists = users.Any(u =>
                u.Name == model.Name &&
                u.Cpf == model.Cpf &&
                u.Birth == model.Birth &&
                u.Unit == model.Unit &&
                u.Tower == model.Tower);

            if (userExists)
            {
                return Ok(new { Status = "Verified" });
            }

            return BadRequest(new { Status = "Not Verified" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Error = ex.Message });
        }
    }
}