using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Resource_Server_3.Controllers;

[ApiController]
[Route("resources")]
public class ResourceController : Controller
{
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetSecretResources()
    {
        var user = HttpContext.User?.Identity?.Name;
        return Ok($"user: {user}");
    }
    
    [Authorize]
    [HttpGet ("/protectWeatherInfo")]
    public async Task<IActionResult> GetWeartherResources()
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
    

    [HttpGet ("/unprotectWeatherInfo")]
    public async Task<IActionResult> GetWeartherInfo()
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