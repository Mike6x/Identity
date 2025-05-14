using Identity.UI.Infrastructure.Api;
using Identity.UI.Infrastructure.Infra;
using Identity.UI.Infrastructure.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.UI.Infrastructure;

public static class ServicesRegistration
{
    public static IServiceCollection RegisterServices(this IServiceCollection services)
    {
        // services.AddScoped<CustomAuthorizationMessageHandler>();
        // services.AddTransient<CustomAuthenticationHandler>();
        // services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
        // services.AddScoped<IJwtTokenService, JwtTokenService>();
        // services.AddScoped<ILoginService, LoginService>();
        // services.AddScoped<ILogoutService, LogoutService>();
        
        // services.AddScoped<IWeatherService, WeatherService>();
        
        services.AddTransient<IApiClient, ApiClient>();
        
        return services;
    }
}