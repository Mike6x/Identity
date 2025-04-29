using Identity.API.Data;

namespace Identity.API.Services;

public static class ServicesRegistration
{
    public static void RegisterServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddTransient<AuthorizationService>();
        builder.Services.AddTransient<ClientSeeder>();
        
    }
}