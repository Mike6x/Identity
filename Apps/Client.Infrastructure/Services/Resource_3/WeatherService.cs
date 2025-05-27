using System.Net.Http.Json;
using Identity.Shared.Resource_Server_3;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace Client.Infrastructure.Services.Resource_3
{
    public interface IWeatherService
    {
        Task<IEnumerable<WeatherForecast>> GetPublicForecastAsync();
        
        Task<IEnumerable<WeatherForecast>> GetSecureForecastAsync();
        
        Task<IEnumerable<WeatherForecast>> GetPaidForecastAsync();
    }


    public class WeatherService(HttpClient httpClient) : IWeatherService
    {
        public async Task<IEnumerable<WeatherForecast>> GetPublicForecastAsync()
        {
            try
            {
                return await httpClient.GetFromJsonAsync<IEnumerable<WeatherForecast>>("api/WeatherForecast/public-forecast")
                       ?? [];
            }
            catch (AccessTokenNotAvailableException ex)
            {
                ex.Redirect();
            }
            return [];
        }
        
        public async Task<IEnumerable<WeatherForecast>> GetSecureForecastAsync()
        {
            try
            {
                return await httpClient.GetFromJsonAsync<IEnumerable<WeatherForecast>>("api/WeatherForecast/sercure-forecast")
                       ?? [];
            }
            catch (AccessTokenNotAvailableException ex)
            {
                ex.Redirect();
            }
            return [];
        }
        
        public async Task<IEnumerable<WeatherForecast>> GetPaidForecastAsync()
        {
            try
            {
                return await httpClient.GetFromJsonAsync<IEnumerable<WeatherForecast>>("api/WeatherForecast/paid-forecast")
                       ?? [];
            }
            catch (AccessTokenNotAvailableException ex)
            {
                ex.Redirect();
            }
            return [];
        }
    }
}
