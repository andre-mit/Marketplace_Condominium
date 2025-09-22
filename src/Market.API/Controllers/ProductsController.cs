using Market.Domain.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Market.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProductsController(ILogger<ProductsController> logger, IProductsRepository productsRepository)
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
}