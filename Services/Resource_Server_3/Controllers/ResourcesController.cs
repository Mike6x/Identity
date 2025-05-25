using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Resource_Server_3.Controllers;

[ApiController]
[Route("[controller]")]

public class ResourcesController : Controller
{
    [HttpGet("logout")]
    public async Task Logout()
    {
        await HttpContext.SignOutAsync("Cookies");
        await HttpContext.SignOutAsync("OpenIddict.Server.AspNetCore");
    }
    
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetCurrentUserName()
    {
        var user = HttpContext.User?.Identity?.Name;
        return Ok($"user: {user}");
    }
    
}