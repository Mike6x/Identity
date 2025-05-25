namespace Resource_Server_3.Models;

/// <summary>
/// Represents the raw data structure for a city as stored in the JSON file.
/// </summary>
public record CityData
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public required string State { get; init; }
    public int SummerHighFahrenheit { get; init; }
    public int WinterLowFahrenheit { get; init; }
    public int ElevationFeet { get; init; }
    public long Population { get; init; }
    public required string TimeZone { get; init; }
}