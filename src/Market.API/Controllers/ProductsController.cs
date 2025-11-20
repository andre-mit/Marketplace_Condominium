using Market.API.Services.Interfaces;
using Market.Domain.Repositories;
using Market.SharedApplication.ViewModels.ProductViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Market.API.Helpers.StringExtensions;

namespace Market.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProductsController(
    ILogger<ProductsController> logger,
    IProductsRepository productsRepository,
    IProductService productService)
    : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAllProducts([FromQuery] int page = 1,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await productsRepository.GetAvailableProductsAsync(page, 10, cancellationToken);
            Response.Headers.Append("X-Total-Count", response.TotalCount.ToString());

            logger.LogDebug("Fetched {Count} products", response.TotalCount);

            return Ok(response.Items);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error fetching products");
            return StatusCode(500, "Internal server error");
        }
    }
    
    [HttpGet("{productId:int}")]
    public async Task<IActionResult> GetProductById([FromRoute] int productId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var product = await productsRepository.GetProductByIdAsync(productId, cancellationToken);
            if (product == null)
            {
                logger.LogWarning("Product with ID {ProductId} not found", productId);
                return NotFound();
            }

            logger.LogDebug("Fetched product with ID {ProductId}", productId);
            return Ok(product);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error fetching product with ID {ProductId}", productId);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> CreateProduct([FromForm] CreateProductViewModel<IFormFileCollection> model,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = GetUserIdFromClaims();
            if (userId == null)
                return Unauthorized("Invalid user ID");

            var productId = await productService.CreateProductAsync(model, userId.Value, cancellationToken);

            logger.LogInformation("Created product with ID {ProductId}", productId);
            return Created();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating product");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPut("{productId:int}")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UpdateProduct(int productId, [FromForm] UpdateProductViewModel<IFormFileCollection> model,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = GetUserIdFromClaims();
            if (userId == null)
                return Unauthorized("Invalid user ID");

            await productService.UpdateProductAsync(productId, userId.Value, model, cancellationToken);

            logger.LogInformation("Updated product with ID {ProductId}", productId);
            return NoContent();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating product with ID {ProductId}", productId);
            return StatusCode(500, "Internal server error");
        }
    }
    
    private Guid? GetUserIdFromClaims()
    {
        var userId = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
        if (userId.IsNullOrWhiteSpace() || !Guid.TryParse(userId, out var userIdGuid))
            return null;

        return userIdGuid;
    }
}