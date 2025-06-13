using Identity.Shared.Authorization;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Validation.AspNetCore;
using Resource_Server_2.Configurations;
using Resource_Server_2.Endpoints;
using Scalar.AspNetCore;

namespace Resource_Server_2;

internal static class HostingExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
    {
 
        services.AddControllers();
        
        services.AddOpenApi();
        
        services.AddOpenIdDictConfig(configuration);
        
        services.AddAuthentication(OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme);
        
        services.AddAuthorizationBuilder()
            .AddPolicy(AppPolicies.CanManageCities, policy => policy.RequireRole(AppRoles.Manager));
        services.AddAuthorizationBuilder()
            .AddPolicy(AppPolicies.CanManageStudents, policy => policy.RequireRole(AppRoles.Manager));
        services.AddAuthorizationBuilder()
            .AddPolicy(AppPolicies.PaidForecast, policy => policy.RequireRole(AppRoles.Admin));
        
        // services.AddAuthorizationBuilder().AddPolicy(AppScopes.WeatherReadScope, 
        //     policy => policy.RequireClaim(ClaimConstants.Permissions, AppScopes.WeatherReadScope));
        
        return services;
    }

    public static WebApplication UsePipeline(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            
            // app.MapScalarApiReference()
            app.MapScalarApiReference(options => options
                .AddPreferredSecuritySchemes("OAuth2")
                .AddAuthorizationCodeFlow("OAuth2", flow =>
                {
                    flow.ClientId = "mvc-client";
                    flow.ClientSecret = "388D45FA-B36B-4988-BA59-B187D329C205";
                    flow.Pkce = Pkce.Sha256;
                    flow.SelectedScopes = ["profile", "email", "api"];
                }));
        }
        
        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapResourceEndpoints();

        app.MapControllers();

        return app;
    }
}