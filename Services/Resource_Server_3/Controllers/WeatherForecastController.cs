using Identity.Shared.Authorization;
using Identity.Shared.Resource_Server_3;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Resource_Server_3.Models;

namespace Resource_Server_3.Controllers;

public class WeatherForecastController(ILogger<WeatherForecastController> logger) : ApiControllerBase
{
    [HttpGet("public-forecast")]
    [AllowAnonymous]
    public IActionResult GetPublicForecast() => Ok(WeatherForecasts);
    
    [HttpGet("secure-forecast")]
    [Authorize]
    public IActionResult GetSecureForecast() => Ok(WeatherForecasts);

    [HttpGet("paid-forecast")]
    [Authorize(AppPolicies.PaidForecast)]
    public IActionResult GetPaidForecast()
    {
        var forecast = WeatherForecasts;
        return Ok(forecast);
    }

    private static readonly string[] Summaries =
    [
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    ];
    
    private static readonly WeatherForecast[] WeatherForecasts =
        Enumerable.Range(1, 5)
            .Select(index => new WeatherForecast()
            {
                Id = index,
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();

    private readonly ILogger<WeatherForecastController> _logger = logger;
}
    

