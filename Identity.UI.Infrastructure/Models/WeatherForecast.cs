namespace Identity.UI.Infrastructure.Models;

public record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary, string? Recommendation)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}