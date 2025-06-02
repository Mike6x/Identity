using Identity.Shared.Authorization;
using Identity.Shared.Resource_Server_2;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Resource_Server_2.Controllers;

public class WeatherForecastController : ApiControllerBase
{
    [HttpGet("public-forecast")]
    [AllowAnonymous]
    public IActionResult GetPublicForecast() => Ok(WeatherForecasts);
    
    [HttpGet("secure-forecast")]
    [Authorize]
    public IActionResult GetSecureForecast() => Ok(WeatherForecasts);
    
    [HttpGet("paid-forecast")]
    [Authorize(Policy = AppScopes.WeatherReadScope)]
    public IActionResult GetPaidForecast() => Ok(WeatherForecasts);

    //Anonymous on purpose to test api gateway also for anonymous endpoints
    [HttpGet("predictions/{id}")]
    [AllowAnonymous]
    public IActionResult GetById([FromRoute] int id) => Ok(WeatherForecasts.First(x => x.Id == id));
    
    
    private static readonly string[] Summaries = [ "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching" ];

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
    
}