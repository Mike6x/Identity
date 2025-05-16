using Blazor.Wasm.Client.Security;
using Client.Infrastructure.Services;
using Client.Infrastructure.Services.Resource_3;

namespace Blazor.Wasm.Client.Configurations;

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
