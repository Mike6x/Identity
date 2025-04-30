using Identity.Infrastructure.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Infrastructure.Services;

public static class ServicesRegistration
{
    public static void RegisterServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddTransient<AuthorizationService>();
        builder.Services.AddTransient<ClientSeeder>();
        
    }
}