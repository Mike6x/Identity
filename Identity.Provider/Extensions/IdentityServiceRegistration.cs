using Identity.Infrastructure.Data.Workers;
using Identity.Infrastructure.Extensions;
using Identity.Provider.Configurations;

namespace Identity.Provider.Extensions;

public static class IdentityServiceRegistration
{
    public static IServiceCollection AddAuthServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDatabaseConfig(configuration);
        
        services.AddIdentityConfig(configuration);
        
        services.AddOpenIdDictConfig(configuration);
        
        services.AddCorsPolicy(configuration);

        services.AddSwaggerConfig(configuration);

        services.AddInfraServices(configuration);
        
        services.AddAntiforgery();
        
        services.AddOpenApi();
        
        services.AddHostedService<OpenIdDictWorker>();
        
        return services;
    }
}