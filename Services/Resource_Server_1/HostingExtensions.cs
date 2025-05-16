using OpenIddict.Validation.AspNetCore;
using Resource_Server_1.Configurations;
using Resource_Server_1.Endpoints;

namespace Resource_Server_1;

internal static class HostingExtensions
{
    // private static IWebHostEnvironment _env;
    
    public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOpenApi();
        services.AddSwaggerConfig(configuration);

        services.AddOpenIdDictConfig(configuration);

        services.AddAuthentication(OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme);
        services.AddAuthorization();
        
        return services;
    }

    public static WebApplication UsePipeline(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseSwaggerService();

        var resourceGroup = app.MapGroup("resources").WithTags("resources server 1");
        resourceGroup.MapResourceEndpoints();

        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();
        
        return app;
    }
}