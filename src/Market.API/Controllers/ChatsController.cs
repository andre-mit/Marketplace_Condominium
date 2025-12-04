using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Market.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ChatsController(ILogger<ChatsController> logger, IChatService chatService) : ControllerBase
{
    [HttpGet("{sessionId:guid}")]
    public async Task<IActionResult> GetChat([FromRoute] Guid sessionId, CancellationToken cancellationToken = default)
    {
        try
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (userId.IsNullOrWhiteSpace() || !Guid.TryParse(userId, out var userIdGuid))
                return Unauthorized("Invalid user ID");

            var chat = await chatService.GetChatSessionAsync(sessionId, userIdGuid, cancellationToken);

            logger.LogInformation("User {UserId} retrieved chat {ChatId}", User.Identity?.Name, userId);

            return Ok(chat);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving chat for user {UserId}", User.Identity?.Name);
            return StatusCode(500, "An error occurred while retrieving the chat.");
        }
    }

    [HttpPost("sync")]
    public async Task<IActionResult> Sync([FromQuery] DateTime? after)
    {
        try
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (userId.IsNullOrWhiteSpace() || !Guid.TryParse(userId, out var userIdGuid))
                return Unauthorized("Invalid user ID");

            var result = await chatService.SyncChatsAsync(userIdGuid, after);

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
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (userId.IsNullOrWhiteSpace() || !Guid.TryParse(userId, out var userIdGuid))
                return Unauthorized("Invalid user ID");

            var chatId = await chatService.CreateChatAsync(userIdGuid, productId, cancellationToken);

            logger.LogInformation("User {UserId} created chat {ChatId}", userId, chatId);

            return Created("chats/" + chatId, new { Id = chatId });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating chat for user {UserId}", User.Identity?.Name);
            return StatusCode(500, "An error occurred while creating the chat.");
        }
    }
}