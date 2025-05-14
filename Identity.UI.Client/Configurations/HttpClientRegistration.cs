using System.Globalization;
using Identity.UI.Client.Security;
using Identity.UI.Infrastructure.Infra;
using Identity.UI.Infrastructure.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace Identity.UI.Client.Configurations;

public static class HttpClientRegistration
{
    public static IServiceCollection RegisterHttpClient(this IServiceCollection services, string authorityUrl)
    {
        services.AddScoped<CustomAuthorizationMessageHandler>();
        
        services.AddHttpClient("WeatherForecast", client =>
            {
                client.BaseAddress = new Uri("https://localhost:7203/"); 
            })
            .AddHttpMessageHandler<CustomAuthorizationMessageHandler>();

        // Supply HttpClient instances that include access tokens when making requests to the server project
        services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("WeatherForecast"));

        services.AddHttpClient<IWeatherService, WeatherService>(client =>
            {
                client.BaseAddress = new Uri("https://localhost:7203/"); 
            })
            .AddHttpMessageHandler<CustomAuthorizationMessageHandler>();
        
        return services;
    }
}
