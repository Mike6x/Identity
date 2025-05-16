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
    
    [Authorize]
    [HttpGet ("secure-weather-forecast")]
    public async Task<IActionResult> GetSecureWeartherForecast()
    {
        var summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };
        
        var forecast = Enumerable.Range(1, 5).Select(index =>
                new WeatherForecast
                (
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    Random.Shared.Next(-20, 55),
                    summaries[Random.Shared.Next(summaries.Length)], "You should stay home"
                ))
            .ToArray();
        
        return Ok(forecast);
    }
    

    [HttpGet ("weather-forecast")]
    public async Task<IActionResult> GetWeartherForecast()
    {
        var summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };
        
        var forecast = Enumerable.Range(1, 5).Select(index =>
                new WeatherForecast
                (
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    Random.Shared.Next(-20, 55),
                    summaries[Random.Shared.Next(summaries.Length)], "You should stay home"
                ))
            .ToArray();
        
        return Ok(forecast);
    }
    
    private record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary, string? Recommendation)
    {
        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
    }
}