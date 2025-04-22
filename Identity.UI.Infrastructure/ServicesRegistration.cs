using Identity.UI.Infrastructure.Api;
using Identity.UI.Infrastructure.Infra;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.UI.Infrastructure;

public static class ServicesRegistration
{
    public static void RegisterServices(this IServiceCollection services)
    {
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<ILoginService, LoginService>();
        services.AddScoped<ILogoutService, LogoutService>();
        
        services.AddTransient<IApiClient, ApiClient>();
    }
}