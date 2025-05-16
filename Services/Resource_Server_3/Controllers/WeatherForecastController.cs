using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Resource_Server_3.Configurations;
using Resource_Server_3.Models;

namespace Resource_Server_3.Controllers;

/// <summary>
/// We require authorized access with policy ReadWeatherDataPolicy.
/// This policy is configured during startup.
/// </summary>
[Authorize(Policy = Policies.ReadWeatherDataPolicy)]

[ApiController]
[Route("api/[controller]")]
public class WeatherForecastController(ILogger<WeatherForecastController> logger) : ControllerBase
{
    private static readonly string[] Summaries =
    [
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    ];

    private readonly ILogger<WeatherForecastController> _logger = logger;

    [HttpGet(Name = "GetWeatherForecast")]
    [Authorize(Policies.ReadWeatherDataPolicy)]
    public IEnumerable<WeatherForecast> Get()
    {
        var forecast = Enumerable.Range(1, 5).Select(index =>
                new WeatherForecast
                (
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    Random.Shared.Next(-20, 55),
                    Summaries[Random.Shared.Next(Summaries.Length)], "You should stay home"
                ))
            .ToArray();
        
        return forecast;
    }
}
