using Market.SharedApplication.ViewModels.TransactionVIewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Market.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TransactionsController(ILogger<TransactionsController> logger, ITransactionService transactionService)
    : ControllerBase
{
    [HttpGet("{transactionId:guid}")]
    public async Task<IActionResult> GetTransaction(Guid transactionId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var transaction = await transactionService.GetTransactionAsync(transactionId, cancellationToken);
            return Ok(transaction);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving transaction");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateTransaction(CreateTransactionViewModel model)
    {
        try
        {
            var userIdClaim = User.Claims
                .FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrWhiteSpace(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                return Unauthorized("Invalid user ID");

            await transactionService.CreateTransactionAsync(userId, model.TransactionType, model.ProductId);

            return Ok("Transaction created successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating transaction");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost("{transactionId:guid}/validate")]
    public async Task<IActionResult> ValidateTransaction(Guid transactionId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var userIdClaim = User.Claims
                .FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrWhiteSpace(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                return Unauthorized("Invalid user ID");

            await transactionService.ValidateTransactionAsync(userId, transactionId, cancellationToken);

            return Ok("Transaction validated successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error validating transaction");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost("{transactionId:guid}/rate")]
    public async Task<IActionResult> RateTransaction(Guid transactionId, RateTransactionViewModel model,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var userIdClaim = User.Claims
                .FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrWhiteSpace(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                return Unauthorized("Invalid user ID");

            await transactionService.RateTransactionAsync(transactionId, userId, model.Review, model.Score,
                cancellationToken);

            return Ok("Transaction rated successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error rating transaction");
            return StatusCode(500, "Internal server error");
        }
    }
}