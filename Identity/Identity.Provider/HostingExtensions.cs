using Identity.Infrastructure.Data.Workers;
using Identity.Infrastructure.Extensions;
using Identity.Provider.Configurations;
using Identity.Provider.EndPoints;
using Identity.Provider.EndPoints.Authentication;
using Identity.Provider.EndPoints.Authorization;
using Identity.Provider.EndPoints.ExternalLogin;
using Identity.Provider.EndPoints.Users;

namespace Identity.Provider;

internal static class HostingExtensions
{
    // private static IWebHostEnvironment _env;
    
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

    public static WebApplication UseAuthPipeline(this WebApplication app)
    {
        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseMigrationsEndPoint();
        }
        else
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }
        
        app.UseSwagger();
        
        app.UseUrlsFromConfig();
        app.UseReverseProxySupport();
        
        app.UseHttpsRedirection();
        app.UseRouting();
        
        app.UseAntiforgery();
        app.UseCorsPolicy();
        app.UseAuthentication();
        app.UseAuthorization();

        var callbackGroup = app.MapGroup("api/callback").WithTags("External Logins");
        callbackGroup.MapExternalCallbackEndpoints();
        
        var authGroup = app.MapGroup("api/auth").WithTags("Authentications");
        authGroup.MapIdentityEndpoints();
        
        var userGroup = app.MapGroup("api/users").WithTags("Users");
        userGroup.MapUsersEndpoints();
        
        var statusGroup = app.MapGroup("api/status").WithTags("Status");
        statusGroup.MapStatusEndpoints();
        
        var authorizationGroup = app.MapGroup("connect").WithTags("Authorizations");
        authorizationGroup.MapOpenIdConnectEndpoints();
        

        // app.UseVueFallbackSpa();
        
        app.MapStaticAssets();
        app.MapRazorPages()
            .WithStaticAssets();
        app.MapControllers();
        
        return app;
    }
    
}