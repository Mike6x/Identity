using Blazored.LocalStorage;
using Client.Infrastructure;
using Identity.Admin.Components;
using Identity.Admin.Configurations;
using Identity.Admin.Endpoints;
using Identity.Admin.Preferences;
using Identity.Shared.Authorization;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Logging;
using MudBlazor;
using MudBlazor.Services;

namespace Identity.Admin;

internal static class HostingExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
    {
        services.AddBlazoredLocalStorage();
        
        services.AddMudServices(config =>
        {
            config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.TopRight;
            config.SnackbarConfiguration.PreventDuplicates = false;
            config.SnackbarConfiguration.NewestOnTop = false;
            config.SnackbarConfiguration.ShowCloseIcon = true;
            config.SnackbarConfiguration.VisibleStateDuration = 4000;
            config.SnackbarConfiguration.HideTransitionDuration = 500;
            config.SnackbarConfiguration.ShowTransitionDuration = 500;
        });
        
        services.AddRazorComponents()
            .AddInteractiveServerComponents();
        
        services.AddHttpContextAccessor();
        
        services.AddOidcConfig(configuration , environment);
        
        services.AddAuthenticationCore();
        
        services.AddAuthorizationCore(options =>
        {
            // options.AddPolicy(AppScopes.UserReadScope, policy => 
            //     policy.RequireClaim(ClaimConstants.Permissions, AppScopes.UserReadScope));
            //
            options.AddPolicy(AppScopes.WeatherReadScope, policy => 
                policy.RequireRole("Admin"));
            
            options.AddPolicy(AppPolicies.CanManageApplications, policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireClaim(ClaimConstants.ReadWriteClaim, "applications");
            });
            options.AddPolicy(AppPolicies.CanManageScopes, policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireClaim(ClaimConstants.ReadWriteClaim, "scopes");
            });
            options.AddPolicy(AppPolicies.CanManageUsers, policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireClaim(ClaimConstants.ReadWriteClaim, "users");
            });
            options.AddPolicy(AppPolicies.CanManageRoles, policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireClaim(ClaimConstants.ReadWriteClaim, "roles");
            });
        });
        
        services.AddCascadingAuthenticationState();
        
        // services.AddRazorPages().WithRazorPagesRoot("/Components/Pages");
        // services.AddSingleton<LocalWeatherForecastService>();
        
        services.RegisterHttpClient(configuration);
        
        services.AddInfraServices(configuration);
        
        services.AddTransient<IClientPreferenceManager, ClientPreferenceManager>();
        services.AddTransient<IPreference, ClientPreference>();
        // services.AddNotifications();
        
        return services;
    }

    public static WebApplication UsePipeline(this WebApplication app)
    {
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
        
        JsonWebTokenHandler.DefaultInboundClaimTypeMap.Clear();
        // app.UseSecurityHeaders();
        
        app.UseHttpsRedirection();
        app.UseAntiforgery();
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapStaticAssets();
        
        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode()
            .RequireAuthorization();

        app.MapAuthEndpoints();
        
        return app;
    }
}