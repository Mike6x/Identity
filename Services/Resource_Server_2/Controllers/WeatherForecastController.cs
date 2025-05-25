using Identity.Shared.Auth;
using Identity.Shared.Resource_Server_2;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Resource_Server_2.Controllers;

public class WeatherForecastController : ApiControllerBase
{
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


    [HttpGet("predictions")]
    [AllowAnonymous]
    public IActionResult GetAll() => Ok(WeatherForecasts);

    //Anonymous on purpose to test api gateway also for anonymous endpoints
    [HttpGet("predictions/{id}")]
    [Authorize(Policy = PolicyConstants.GetWeather)]

    public IActionResult GetById([FromRoute] int id) => Ok(WeatherForecasts.First(x => x.Id == id));
}