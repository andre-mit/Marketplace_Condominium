using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Market.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ChatsController(ILogger<ChatsController> logger, IChatService chatService) : ControllerBase
{
    public async Task<IActionResult> Sync([FromQuery] DateTime? after)
    {
        try
        {
            var userId = Guid.Parse(User.Identity?.Name ??
                                    throw new InvalidOperationException("User is not authenticated"));
            var result = await chatService.SyncChatsAsync(userId, after);
            
            logger.LogInformation("User {UserId} synced chats after {After}", userId, after);
            
            return Ok(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error syncing chats for user {UserId}", User.Identity?.Name);
            return StatusCode(500, "An error occurred while syncing chats.");
        }
    }
}