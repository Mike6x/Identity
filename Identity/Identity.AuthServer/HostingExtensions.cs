using System.Reflection;
using BuildingBlocks;
using BuildingBlocks.Auth;
using FluentValidation;
using Identity.Core;
using Identity.Infrastructure;
using Identity.Infrastructure.Configurations;
using Identity.Infrastructure.Data.Workers;
using Identity.Infrastructure.Endpoints.HealthCheck;
using Identity.Infrastructure.Services;

namespace Identity.AuthServer;

internal static class HostingExtensions
{
    public static IServiceCollection AddAuthServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Register Fluent validators
        var assemblies = new[]
        {
            typeof(IdentityProvider).Assembly,
            typeof(IdentityCore).Assembly,
            typeof(IdentityInfrastructure).Assembly,
    
        };
        services.AddValidatorsFromAssemblies(assemblies);
        
        
        services.AddBlockServices(configuration);
        
        services.AddDatabaseConfig(configuration);
        
        services.AddIdentityConfig(configuration);
        
        services.AddAuthenticationConfig(configuration);
        
        services.AddOpenIdDictConfig(configuration);
        
        services.AddCorsPolicy(configuration);

        services.AddSwaggerConfig(configuration);

        services.AddInfraServices(configuration);
        
        services.AddAntiforgery();
        
        services.AddOpenApi();
        
        services.AddAuthorizationPolicy();
        
        services.AddHostedService<OpenIdDictWorker>();
        
        return services;
    }

    public static WebApplication UseAuthPipeline(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseMigrationsEndPoint();
        }
        else
        {
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }
        app.RegisterHealthCheckEndpoints();

        app.UseBlockServices();
        
        app.UseSwagger();
        
        app.UseUrlsFromConfig();
        app.UseReverseProxySupport();
        
        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseSession();
        
        app.UseAntiforgery();
        app.UseCorsPolicy();
        app.UseAuthentication();
        app.UseAuthorization();

        // Current user middleware
        app.UseMiddleware<CurrentUserMiddleware>();
        
        app.MapIdentityEndpoints();
        
        app.MapStaticAssets();
        app.MapRazorPages()
            .WithStaticAssets();
        app.MapControllers();
        
        return app;
    }
    
}