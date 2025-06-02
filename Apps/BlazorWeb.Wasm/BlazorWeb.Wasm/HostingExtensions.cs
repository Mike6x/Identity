
using BlazorWeb.Wasm.Client.Pages;
using BlazorWeb.Wasm.Client.Security;
using BlazorWeb.Wasm.Client.Services;
using BlazorWeb.Wasm.Components;
using BlazorWeb.Wasm.Configurations;
using BlazorWeb.Wasm.EndPoints;
using BlazorWeb.Wasm.Services;
using Identity.Shared.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;

namespace BlazorWeb.Wasm;

internal static class HostingExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
    {
        services.AddOidcConfig(configuration, environment);
        
        services.AddRazorComponents()
            .AddAuthenticationStateSerialization()
            .AddInteractiveServerComponents()
            .AddInteractiveWebAssemblyComponents();

        services.AddHttpContextAccessor();
        
        services.AddAuthenticationCore();
        services.AddAuthorizationCore(config =>
        {
            config.AddPolicy(AppScopes.UserReadScope, policy => 
                policy.RequireClaim(ClaimConstants.Permissions, AppScopes.UserReadScope));
            
            // config.AddPolicy(AppScopes.WeatherReadScope, policy => 
            //      policy.RequireClaim(ClaimConstants.Permissions, AppScopes.WeatherReadScope));
            
            config.AddPolicy(AppScopes.WeatherReadScope, policy => 
                policy.RequireRole("Admin"));
        });

        services.AddCascadingAuthenticationState();
        
        services.AddSingleton<IAuthorizationMiddlewareResultHandler, BlazorAuthorizationMiddlewareResultHandler>();
        services.AddScoped<HostingEnvironmentService>();
        services.AddSingleton<BaseUrlProvider>();
        
        services.RegisterHttpClient(configuration);
        
        return services;
    }

    public static WebApplication UsePipeline(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseWebAssemblyDebugging();
        }
        else
        {
            app.UseExceptionHandler("/Error", createScopeForErrors: true);
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseAntiforgery();
        app.UseAuthentication();
        app.UseAuthorization();
        
        app.MapStaticAssets();
        
        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode()
            .AddInteractiveWebAssemblyRenderMode()
            .AddAdditionalAssemblies(typeof(Counter).Assembly);
        
        app.MapAuthEndpoints();
        app.MapGet("/api/Counter", (HttpContext httpContext) => Results.Ok("Hi!"))
            .RequireAuthorization();
        
        return app;
    }
}