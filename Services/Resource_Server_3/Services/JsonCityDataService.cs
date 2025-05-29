using System.Text.Json;
using Identity.Shared.Resource_Server_3.Dtos;
using Resource_Server_3.Models;

namespace Resource_Server_3.Services;

public class JsonCityDataService : ICityService
{
    private List<CityData>? _mockData;
    private readonly string _jsonFilePath;
    private static readonly SemaphoreSlim FileReadLock = new(1, 1); // To prevent race conditions on first load
    
    private readonly ILogger<JsonCityDataService> _logger;
    
    public JsonCityDataService(IHostEnvironment environment, ILogger<JsonCityDataService> logger)
    {
        // Combine ContentRootPath with the relative path to the JSON file
        _jsonFilePath = Path.Combine(environment.ContentRootPath, "Data", "Cities.Api.json");
        _logger = logger;
        _logger.LogInformation("JsonCityDataService initialized. Data path: {FilePath}", _jsonFilePath);
    }

    public async Task<IEnumerable<CityData>> GetAllAsync()
    {
        await EnsureDataLoadedAsync();
        return _mockData ?? Enumerable.Empty<CityData>();
    }
    
    public async Task<IEnumerable<CityData>> GetListAsync()
    {
        await EnsureDataLoadedAsync();
        return _mockData ?? Enumerable.Empty<CityData>();
    }
    
    public async Task<CityData?> GetByCodeAsync(string code)
    {
        await EnsureDataLoadedAsync();
        if (_mockData == null || _mockData.Count == 0)
        {
            _logger.LogWarning("City data is not loaded or is empty when searching for city: {Code}", code);
            return null;
        }

        // Using LINQ's FirstOrDefault for efficient searching.
        // StringComparison.OrdinalIgnoreCase is recommended for case-insensitive comparisons of identifiers.
        var city = _mockData.FirstOrDefault(c => c.Code.Equals(code, StringComparison.OrdinalIgnoreCase));

        if (city == null)
        {
            _logger.LogInformation("City not found: {Code}", code);
        }
        else
        {
            _logger.LogInformation("City found: {Code}", code);
        }
        return city;
    }
    
    public async Task<CityData?> GetByIdAsync(int id)
    {
        await EnsureDataLoadedAsync();
        if (_mockData == null || _mockData.Count == 0)
        {
            _logger.LogWarning("City data is not loaded or is empty when searching for city: {id}", id);
            return null;
        }

        // Using LINQ's FirstOrDefault for efficient searching.
        // StringComparison.OrdinalIgnoreCase is recommended for case-insensitive comparisons of identifiers.
        var city = _mockData?.FirstOrDefault(c => c.Id.Equals(id));
    
        if (city == null)
        {
            _logger.LogInformation("City not found: {id}", id);
        }
        else
        {
            _logger.LogInformation("City found: {id}", id);
        }
        return city;
    }

    public async Task<int?> DeleteAsync(int id)
    {
        await EnsureDataLoadedAsync();
            
        var result = _mockData?.FirstOrDefault(c => c.Id.Equals(id));
            
        if (result == null) return null;
            
        _mockData?.Remove(result);
            
        return result.Id;
    }
    
    public async Task<CityData?> CreateAsync(CityInfoDto cityInfoDto)
    {
        await EnsureDataLoadedAsync();
            
        var result = _mockData?.FirstOrDefault(c => c.Name.Equals(cityInfoDto.Name, StringComparison.OrdinalIgnoreCase));
        if (result != null) return null;
            
        int newId = _mockData?.Max(s => s.Id) + 1 ?? 0;

        var item = new CityData()
        {
            Id = newId,
            Code = cityInfoDto.Code,
            Name = cityInfoDto.Name,
            State = cityInfoDto.State,
            ElevationFeet = cityInfoDto.ElevationFeet,
            Population = cityInfoDto.Population,
            TimeZone = cityInfoDto.TimeZone,
            SummerHighFahrenheit = cityInfoDto.Temperatures.SummerHighFahrenheit,
            WinterLowFahrenheit = cityInfoDto.Temperatures.WinterLowFahrenheit,
        };

        _mockData?.Add(item);
            
        return item;
    }

    public async Task<CityData?> UpdateAsync(CityInfoDto cityInfoDto)
    {
        await EnsureDataLoadedAsync();
            
        var result = _mockData?.FirstOrDefault(c => c.Id.Equals(cityInfoDto.Id));
        if (result == null) return null;
            
        var newItem = new CityData()
        {
            Id = cityInfoDto.Id,
            Code = cityInfoDto.Code,
            Name = cityInfoDto.Name,
            State = cityInfoDto.State,
            ElevationFeet = cityInfoDto.ElevationFeet,
            Population = cityInfoDto.Population,
            TimeZone = cityInfoDto.TimeZone,
            SummerHighFahrenheit = cityInfoDto.Temperatures.SummerHighFahrenheit,
            WinterLowFahrenheit = cityInfoDto.Temperatures.WinterLowFahrenheit,
        };
            
        _mockData?.Remove(result);
            
        _mockData?.Add(newItem);
            
        return newItem;
    }
    

    /// <summary>
    /// Loads city data from the JSON file.
    /// This method is asynchronous and thread-safe for initialization.
    /// </summary>
    private async Task EnsureDataLoadedAsync()
    {
        if (_mockData != null && _mockData.Count != 0) return; // Data already loaded

        await FileReadLock.WaitAsync(); // Acquire lock
        try
        {
            if (_mockData != null && _mockData.Count != 0) return; // Double-check after acquiring lock

            _logger.LogInformation("Attempting to load city data from {FilePath}", _jsonFilePath);
            if (!File.Exists(_jsonFilePath))
            {
                _logger.LogError("City data JSON file not found at {FilePath}", _jsonFilePath);
                _mockData = []; // Initialize with empty list to prevent repeated load attempts
                return;
            }

            var jsonString = await File.ReadAllTextAsync(_jsonFilePath);
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true // Handles different casing in JSON properties
            };
            _mockData = JsonSerializer.Deserialize<List<CityData>>(jsonString, options);
            _logger.LogInformation("Successfully loaded {Count} cities from JSON.", _mockData?.Count ?? 0);
        }
        catch (JsonException jsonEx)
        {
            _logger.LogError(jsonEx, "Error deserializing city data from {FilePath}.", _jsonFilePath);
            _mockData = []; // Initialize with empty list on error
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred while loading city data from {FilePath}.", _jsonFilePath);
            _mockData = []; // Initialize with empty list on error
        }
        finally
        {
            FileReadLock.Release(); // Release lock
        }
    }
}