using Client.Infrastructure.Api;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Client.Infrastructure;

public static class ServicesRegistration
{
    public static IServiceCollection AddInfraServices(this IServiceCollection services, IConfiguration configuration)
    {
        // services.AddScoped<CustomAuthorizationMessageHandler>();
        // services.AddTransient<CustomAuthenticationHandler>();
        // services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
        // services.AddScoped<IJwtTokenService, JwtTokenService>();
        // services.AddScoped<ILoginService, LoginService>();
        // services.AddScoped<ILogoutService, LogoutService>();
        
        services.AddTransient<IApiClient, ApiClient>();
        
        return services;
    }
}