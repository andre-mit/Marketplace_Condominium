using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Market.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ChatsController(ILogger<ChatsController> logger, IChatService chatService) : ControllerBase
{
    [HttpPost("sync")]
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

    [HttpPost("create")]
    public async Task<IActionResult> CreateChat([FromQuery] int productId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var userId = Guid.Parse(User.Identity?.Name ??
                                    throw new InvalidOperationException("User is not authenticated"));
            var chatId = await chatService.CreateChatAsync(userId, productId, cancellationToken);

            logger.LogInformation("User {UserId} created chat {ChatId}", userId, chatId);

            return Ok(new { Id = chatId });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating chat for user {UserId}", User.Identity?.Name);
            return StatusCode(500, "An error occurred while creating the chat.");
        }
    }
}