using Identity.Shared.Resource_Server_2;

namespace Client.Infrastructure.Services.MockData
{
    public class LocalWeatherForecastService
    {
        private static readonly string[] Summaries = 
                ["Đông lạnh", "Khỏe mạnh", "Lạnh", "Mát mẻ", "Nhẹ nhàng", "Ấm áp", "Dịu mát", "Nóng", "Quá ngột ngạt", "Nóng như thiêu đốt"];

        public Task<WeatherForecast[]> GetForecastAsync(DateTime startDate)
        {
            return Task.FromResult(WeatherForecasts);
        }

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
    
}
