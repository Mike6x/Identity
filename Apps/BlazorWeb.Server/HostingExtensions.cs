using BlazorWeb.Server.Components;
using BlazorWeb.Server.Configurations;
using BlazorWeb.Server.Endpoints;
using Client.Infrastructure.Services.MockData;
using Identity.Shared.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Server.Circuits;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Logging;

namespace BlazorWeb.Server;

internal static class HostingExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
    {
        services.AddRazorComponents()
            .AddInteractiveServerComponents();
        
        services.AddHttpContextAccessor();
        
        services.AddOidcConfig(configuration , environment);
        
        services.AddAuthenticationCore();

        services.AddAuthorizationCore(options =>
        {
            options.AddPolicy(AppPolicies.CanManageStudents, policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireRole(AppRoles.Manager);
            });
            
            options.AddPolicy(AppPolicies.PaidForecast, policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireRole(AppRoles.Admin);
            });
            
        });
        
        services.AddCascadingAuthenticationState();
        
        // services.AddRazorPages().WithRazorPagesRoot("/Components/Pages");
        
        services.AddSingleton<LocalWeatherForecastService>();
        
        services.RegisterHttpClient(configuration);
        
        return services;
    }

    public static WebApplication UsePipeline(this WebApplication app)
    {
        JsonWebTokenHandler.DefaultInboundClaimTypeMap.Clear();

        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error", createScopeForErrors: true);
            app.UseHsts();
        }
        else
        {
            IdentityModelEventSource.ShowPII = true;
            IdentityModelEventSource.LogCompleteSecurityArtifact = true;
        }

        app.UseSecurityHeaders();

        app.UseHttpsRedirection();
        app.UseAntiforgery();
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapStaticAssets();

        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode()
            .RequireAuthorization()
            ;
        
        // app.MapLoginLogoutEndpoints();
        app.MapAuthEndpoints();
        
        return app;
    }
    
}