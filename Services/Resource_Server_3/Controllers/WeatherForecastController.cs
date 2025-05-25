using Identity.Shared.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Resource_Server_3.Models;

namespace Resource_Server_3.Controllers;

/// <summary>
/// We require authorized access with policy ReadWeatherDataPolicy.
/// This policy is configured during startup.
/// </summary>
[Authorize(Policy = PolicyConstants.ReadWeatherDataPolicy)]

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
    [Authorize(PolicyConstants.ReadWeatherDataPolicy)]
    public IEnumerable<WeatherForecast> GetWeatherForecast()
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


    [HttpGet("secure-weather-forecast")]
    [Authorize]
    public async Task<IActionResult> GetSecureWeartherForecast()
    {

        var forecast = Enumerable.Range(1, 5).Select(index =>
                new WeatherForecast
                (
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    Random.Shared.Next(-20, 55),
                    Summaries[Random.Shared.Next(Summaries.Length)], "You can gou out"
                ))
            .ToArray();

        return Ok(forecast);
    }


    [HttpGet("weather-forecast")]
    [AllowAnonymous]
    public async Task<IActionResult> GetWeartherForecast()
    {


        var forecast = Enumerable.Range(1, 5).Select(index =>
                new WeatherForecast
                (
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    Random.Shared.Next(-20, 55),
                    Summaries[Random.Shared.Next(Summaries.Length)], "You should stay home"
                ))
            .ToArray();

        return Ok(forecast);
    }
}
    

