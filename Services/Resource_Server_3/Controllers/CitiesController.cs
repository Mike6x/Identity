using System.Globalization;
using Identity.Shared.Resource_Server_3.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Resource_Server_3.Models;
using Resource_Server_3.Services;

namespace Resource_Server_3.Controllers;

public class CitiesController(ICityService cityService, ILogger<CitiesController> logger)
    : ApiControllerBase
{
    private readonly ICityService _cityService = cityService ?? throw new ArgumentNullException(nameof(cityService));
    private readonly ILogger<CitiesController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    
    [HttpPost("GetList")]
    [AllowAnonymous]
    public async Task<IActionResult> GetList()
    {
        var cities = await _cityService.GetAllAsync();
        var cityList = cities.Select(MapToDto).ToList();

        return Ok(cityList);
    }
    
    [HttpGet("code/{code}")]
    [AllowAnonymous]

    public async Task<IActionResult> GetByCodeAsync(string code)
    {
        var result = await cityService.GetByCodeAsync(code);
        
        if (result == null) return NotFound();
        
        return Ok(MapToDto(result));
        
    }
    
    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetByIdAsync(int id)
    {
        var result = await cityService.GetByIdAsync(id);
        
        if (result == null) return NotFound();
        
        return Ok(MapToDto(result));
    }
    
    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> CreateAsync(CityInfoDto itemDto)
    {
        var result = await cityService.CreateAsync(itemDto);
        
        return Ok(result);
    }
    
    [HttpPut("{id:int}")]
    [AllowAnonymous]
    public async Task<IActionResult> UpdateAsync(int id, CityInfoDto? itemDto)
    {
        if (itemDto == null || itemDto.Id < 1 || id != itemDto.Id)
        {
            return BadRequest("Invalid data.");
        }
        var result = await cityService.UpdateAsync(itemDto);
        
        return Ok(result);
    }
    
    [HttpDelete("{id:int}")]
    [AllowAnonymous]
    public async Task<IActionResult> DeleteAsync(int id)
    {
        var result = await  cityService.DeleteAsync(id);
        
        return Ok(result);
    }
    
    
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<object>>> GetAll()
    {
        _logger.LogInformation("Attempting to retrieve all city names.");
        var cities = await _cityService.GetAllAsync();
        var cityList = cities.Select(MapToDto).ToList();

        _logger.LogInformation("Retrieved {Count} Cities.Api.", cityList.Count);
        return Ok(cityList);
    }

    
    [HttpGet("name/{name}")]
    [ProducesResponseType(typeof(CityInfoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    
    public async Task<ActionResult<CityInfoDto>> GetByName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            _logger.LogWarning("City name parameter was null or whitespace.");
            return BadRequest("City name cannot be empty.");
        }

        _logger.LogInformation("Attempting to retrieve information for city: {name}", name);

        var cityData = await _cityService.GetByCodeAsync(name);

        if (cityData == null)
        {
            _logger.LogWarning("City not found: {name}", name);
            return NotFound($"Information for city '{name}' not found.");
        }

        // Map CityData to CityInfoDto (Manual mapping for clarity, AutoMapper could be used in larger projects)
        var cityInfoDto = MapToDto(cityData);

        _logger.LogInformation("Successfully retrieved information for city: {CityName}", name);
        return Ok(cityInfoDto);
    }
    
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
            Id = cityData.Id,
            Code = cityData.Code,
            Name = cityData.Name,
            State = cityData.State,
            Temperatures = new TemperatureInfo
            {
                SummerHighFahrenheit = cityData.SummerHighFahrenheit,
                WinterLowFahrenheit = cityData.WinterLowFahrenheit
                // SummerHighFahrenheit = $"{cityData.SummerHighFahrenheit} °F",
                // WinterLowFahrenheit = $"{cityData.WinterLowFahrenheit} °F"
            },
            // ElevationFeet = $"{cityData.ElevationFeet} ft",
            
            ElevationFeet =  cityData.ElevationFeet,
            Population = cityData.Population,
            TimeZone = currentTimeLocal
        };
    }
}