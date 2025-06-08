namespace Identity.Shared.Resource_Server_3.Dtos;

public record CityInfoDto
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public int ElevationFeet { get; set; }
    public long Population { get; set; }
    
    public string TimeZone { get; set; } = string.Empty;  

    public TemperatureInfo Temperatures { get; set; } = new TemperatureInfo
    {
        SummerHighFahrenheit = 0,
        WinterLowFahrenheit = 0
    };
}


public class TemperatureInfo
{
    public int SummerHighFahrenheit { get; set; } 
    public int WinterLowFahrenheit { get; set; }
}