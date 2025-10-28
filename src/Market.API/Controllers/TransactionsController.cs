using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Market.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TransactionsController: ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateTransaction()
    {
        // Implementation for creating a transaction
        return Ok();
    }
}