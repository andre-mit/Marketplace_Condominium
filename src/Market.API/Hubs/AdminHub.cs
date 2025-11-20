using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Market.API.Hubs;

[Authorize(Roles = "Admin")]
public class AdminHub(ILogger<AdminHub> logger) : Hub
{
}