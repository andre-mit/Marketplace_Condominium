using System.Security.Claims;
using Market.API.Services.Interfaces;
using Market.Domain.Enums;
using Market.Domain.Repositories;
using Market.SharedApplication.ViewModels.CategoryViewModels;
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
    IProductService productService)
    : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAllProducts(
        [FromQuery] int page = 1, [FromQuery] int pageSize = 10,
        [FromQuery] string? searchTerm = null,
        [FromQuery] int? categoryId = null,
        [FromQuery] TransactionType? transactionType = null,
        [FromQuery] ProductCondition? condition = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var response =
                await productService.ListProductsAsync(page, pageSize, searchTerm, categoryId, transactionType,
                    condition, cancellationToken);

            logger.LogDebug("Fetched {Count} products", response.TotalCount);

            return Ok(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error fetching products");
            return StatusCode(500, "Internal server error");
        }
    }
    
    [HttpGet("mine")]
    public async Task<IActionResult> GetMyProducts(
        [FromQuery] int page = 1, [FromQuery] int pageSize = 10,
        [FromQuery] string? searchTerm = null,
        [FromQuery] int? categoryId = null,
        [FromQuery] TransactionType? transactionType = null,
        [FromQuery] ProductCondition? condition = null,
        [FromQuery] bool? isAvailable = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var userId = GetUserIdFromClaims();
            if (userId == null)
                return Unauthorized("Invalid user ID");

            var response =
                await productService.ListMineProductsAsync(userId.Value, page, pageSize, searchTerm, categoryId,
                    transactionType,
                    condition, isAvailable, cancellationToken);
            
            logger.LogDebug("Fetched {Count} products for user {UserId}", response.TotalCount, userId);
            return Ok(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error fetching user's products");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("categorized")]
    public async Task<IActionResult> GetCategorizedProducts([FromQuery] int limitByCategory = 5,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var categorizedProducts =
                await productService.ListCategorizedProductsAsync(limitByCategory, cancellationToken);
            logger.LogDebug("Fetched categorized products with limit {LimitByCategory}", limitByCategory);
            return Ok(categorizedProducts);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error fetching categorized products");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("{productId:int}")]
    public async Task<IActionResult> GetProductById([FromRoute] int productId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var product = await productService.GetProductByIdAsync(productId, cancellationToken);
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
    [ProducesResponseType(204)]
    public async Task<IActionResult> UpdateProduct(int productId,
        [FromForm] UpdateProductViewModel<IFormFileCollection> model,
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

    [HttpGet("categories")]
    public async Task<ActionResult<List<ListCategoryViewModel>>> GetProductCategories(
        CancellationToken cancellationToken = default)
    {
        try
        {
            var categories = await productService.GetAllCategoriesAsync(cancellationToken);
            logger.LogDebug("Fetched {Count} product categories", categories.Count);
            return Ok(categories);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error fetching product categories");
            return StatusCode(500, "Internal server error");
        }
    }

    private Guid? GetUserIdFromClaims()
    {
        var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        if (userId.IsNullOrWhiteSpace() || !Guid.TryParse(userId, out var userIdGuid))
            return null;

        return userIdGuid;
    }
}