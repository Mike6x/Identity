using System.Net.Http.Json;
using Identity.UI.Infrastructure.Models;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace Identity.UI.Infrastructure.Services
{
    public interface IWeatherService
    {
        Task<IEnumerable<WeatherForecast>> GetAllAsync();
        
        Task<IEnumerable<WeatherForecast>> GetWeatherInfoAsync();
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
        
        public async Task<IEnumerable<WeatherForecast>> GetWeatherInfoAsync()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<IEnumerable<WeatherForecast>>("resources/WeatherInfo")
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
