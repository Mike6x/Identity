using System.Globalization;
using Identity.Shared.Resource_Server_3.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Resource_Server_3.Models;
using Resource_Server_3.Services;

namespace Resource_Server_3.Controllers;

public class CitiesController(ICityDataService cityDataService, ILogger<CitiesController> logger)
    : ApiControllerBase
{
    private readonly ICityDataService _cityDataService = cityDataService ?? throw new ArgumentNullException(nameof(cityDataService));
    private readonly ILogger<CitiesController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));


    [HttpGet("{cityName}")]
    [ProducesResponseType(typeof(CityInfoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    
    public async Task<ActionResult<CityInfoDto>> GetCityInfo(string cityName)
    {
        if (string.IsNullOrWhiteSpace(cityName))
        {
            _logger.LogWarning("City name parameter was null or whitespace.");
            return BadRequest("City name cannot be empty.");
        }

        _logger.LogInformation("Attempting to retrieve information for city: {CityName}", cityName);

        var cityData = await _cityDataService.GetByNameAsync(cityName);

        if (cityData == null)
        {
            _logger.LogWarning("City not found: {CityName}", cityName);
            return NotFound($"Information for city '{cityName}' not found.");
        }

        // Map CityData to CityInfoDto (Manual mapping for clarity, AutoMapper could be used in larger projects)
        var cityInfoDto = MapToDto(cityData);

        _logger.LogInformation("Successfully retrieved information for city: {CityName}", cityName);
        return Ok(cityInfoDto);
    }


    [HttpGet]
    [AllowAnonymous]
    // [ProducesResponseType(typeof(IEnumerable<object>), StatusCodes.Status200OK)]
    
    public async Task<ActionResult<IEnumerable<object>>> GetAllCityNames()
    {
        _logger.LogInformation("Attempting to retrieve all city names.");
        var cities = await _cityDataService.GetAllAsync();
        var cityList = cities.Select(MapToDto).ToList();
        //  var cityList = cities.Select(c => new { c.Id, c.Name, c.State }).ToList();
        _logger.LogInformation("Retrieved {Count} Cities.Api.", cityList.Count);
        return Ok(cityList);
    }


    /// <summary>
    /// Maps a <see cref="CityData"/> object to a <see cref="CityInfoDto"/> object.
    /// This includes formatting and calculating derived data like current local time.
    /// </summary>
    /// <param name="cityData">The source <see cref="CityData"/> object.</param>
    /// <returns>The mapped <see cref="CityInfoDto"/> object.</returns>
    private CityInfoDto MapToDto(CityData cityData)
    {
        string currentTimeLocal = "N/A";
        try
        {
            var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(cityData.TimeZone);
            var utcNow = DateTime.UtcNow;
            var localTime = TimeZoneInfo.ConvertTimeFromUtc(utcNow, timeZoneInfo);
            currentTimeLocal = localTime.ToString("yyyy-MM-dd HH:mm:ss zzz", CultureInfo.InvariantCulture);
        }
        catch (TimeZoneNotFoundException tzex)
        {
            _logger.LogWarning(tzex, "Time zone ID '{TimeZoneId}' for city '{CityName}' not found on this system.",
                cityData.TimeZone, cityData.Name);
            currentTimeLocal = $"Invalid TimeZoneId: {cityData.TimeZone}";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating local time for city: {CityName}", cityData.Name);
        }

        return new CityInfoDto
        {
            Name = cityData.Name,
            State = cityData.State,
            Temperatures = new TemperatureInfo
            {
                SummerHighFahrenheit = $"{cityData.SummerHighFahrenheit} °F",
                WinterLowFahrenheit = $"{cityData.WinterLowFahrenheit} °F"
            },
            Elevation = $"{cityData.ElevationFeet} ft",
            Population = cityData.Population,
            CurrentTimeLocal = currentTimeLocal
        };
    }
}