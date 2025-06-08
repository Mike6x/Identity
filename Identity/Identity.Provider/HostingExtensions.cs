using BuildingBlocks;
using Identity.Infrastructure.Data.Workers;
using Identity.Infrastructure.Services;
using Identity.Provider.Configurations;
using Identity.Provider.EndPoints;

namespace Identity.Provider;

internal static class HostingExtensions
{
    // private static IWebHostEnvironment _env;
    
    public static IServiceCollection AddAuthServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddBlockServices(configuration);
        
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

        app.UseBlockServices();
        
        app.UseSwagger();
        
        app.UseUrlsFromConfig();
        app.UseReverseProxySupport();
        
        app.UseHttpsRedirection();
        app.UseRouting();
        
        app.UseAntiforgery();
        app.UseCorsPolicy();
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapIdentityEndpoints();
        
        app.MapStaticAssets();
        app.MapRazorPages()
            .WithStaticAssets();
        app.MapControllers();
        
        return app;
    }
    
}