using System.Net.Http.Json;
using Client.Infrastructure.Models;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace Client.Infrastructure.Services.Resource_3
{
    public interface IWeatherService
    {
        Task<IEnumerable<WeatherForecast>> GetAllAsync();
        
        Task<IEnumerable<WeatherForecast>> GetWeatherForcastAsync();
        
        Task<IEnumerable<WeatherForecast>> GetSecureWeatherForcastAsync();
    }

    public class WeatherService : IWeatherService
    {
        private readonly HttpClient _httpClient;

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="httpClient"></param>
        public WeatherService(HttpClient httpClient)
        {
            this._httpClient = httpClient;
        }

        public async Task<IEnumerable<WeatherForecast>> GetAllAsync()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<IEnumerable<WeatherForecast>>("api/WeatherForecast")
                      ?? [];
            }
            catch (AccessTokenNotAvailableException ex)
            {
                ex.Redirect();
            }
            return [];
        }
        
        public async Task<IEnumerable<WeatherForecast>> GetWeatherForcastAsync()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<IEnumerable<WeatherForecast>>("resources/weather-forecast")
                       ?? [];
            }
            catch (AccessTokenNotAvailableException ex)
            {
                ex.Redirect();
            }
            return [];
        }
        
        public async Task<IEnumerable<WeatherForecast>> GetSecureWeatherForcastAsync()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<IEnumerable<WeatherForecast>>("resources/secure-weather-forecast")
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
