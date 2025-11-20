using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Market.API.Controllers.Admin;

[ApiController]
[Route("api/admin/[controller]")]
[Authorize(Roles = "Admin")]
public class UsersController(ILogger<UsersController> logger, IUsersRepository usersRepository) : ControllerBase
{
    [HttpGet("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public IActionResult GetById(Guid id)
    {
        var user = usersRepository.GetById(id);
        if (user == null)
            return NotFound("User not found");

        return Ok(user);
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllUsers([FromQuery] int page = 1,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await usersRepository.GetAllUsersAsync(page, 10, cancellationToken);
            Response.Headers.Append("X-Total-Count", response.TotalCount.ToString());
            
            logger.LogDebug("Fetched {Count} users", response.TotalCount);
            
            return Ok(response.Items);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error fetching users");
            return StatusCode(500, "Internal server error");
        }
    }
}