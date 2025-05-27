namespace Identity.Shared.Resource_Server_3.Dtos;

/// <summary>
/// Data Transfer Object representing city information returned by the API.
/// </summary>
public record CityInfoDto
{
    public required string Name { get; init; }
    public required string State { get; init; }
    public required TemperatureInfo Temperatures { get; init; }
    public required string Elevation { get; init; }
    public long Population { get; init; }
    public required string CurrentTimeLocal { get; init; } // Added feature
}

/// <summary>
/// Temperature information for a city.
/// </summary>
public record TemperatureInfo
{
    public required string SummerHighFahrenheit { get; init; }
    public required string WinterLowFahrenheit { get; init; }
}