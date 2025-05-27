namespace Identity.Shared.Resource_Server_3;

public class WeatherForecast
{
    public int Id { get; set; }

    public DateOnly Date { get; set; }

    public int TemperatureC { get; set; }

    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

    public string? Summary { get; set; }
}

// public record WeatherForecast(int Id, DateOnly Date, int TemperatureC, string? Summary, string? Recommendation)
// {
//     public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
// }