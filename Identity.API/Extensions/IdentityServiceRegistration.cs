using Identity.API.Configurations;
using Identity.API.Workers;

namespace Identity.API.Extensions;

public static class IdentityServiceRegistration
{
    public static IServiceCollection AddAuthServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDatabaseConfig(configuration);
        
        services.AddIdentityConfig(configuration);
        
        services.AddOpenIdDictConfig(configuration);
        
        services.AddCorsPolicy(configuration);

        services.AddSwaggerConfig(configuration);


        services.AddAntiforgery();
        
        services.AddOpenApi();
        
        services
            .AddHostedService<OpenIdDictWorker>();
        
        //builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();
        return services;
    }
}