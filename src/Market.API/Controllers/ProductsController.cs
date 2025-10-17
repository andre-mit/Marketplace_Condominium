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
    [HttpGet("{page:int}")]
    public async Task<IActionResult> GetAllProducts([FromRoute] int page = 1,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var (products, total) = await productsRepository.GetAllProductsAsync(page, 10, cancellationToken);
            Response.Headers.Append("X-Total-Count", total.ToString());

            logger.LogDebug("Fetched {Count} products", products.Count);

            return Ok(products);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error fetching products");
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

            var userId = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
            if (userId.IsNullOrWhiteSpace() || !Guid.TryParse(userId, out var userIdGuid))
                return Unauthorized("Invalid user ID");
            
            var productId = await productService.CreateProductAsync(model, userIdGuid, cancellationToken);

            logger.LogInformation("Created product with ID {ProductId}", productId);
            return Created();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating product");
            return StatusCode(500, "Internal server error");
        }
    }
}