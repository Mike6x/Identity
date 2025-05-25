
using BlazorWeb.Wasm.Client.Pages;
using BlazorWeb.Wasm.Client.Security;
using BlazorWeb.Wasm.Client.Services;
using BlazorWeb.Wasm.Components;
using BlazorWeb.Wasm.Configurations;
using BlazorWeb.Wasm.EndPoints;
using BlazorWeb.Wasm.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;

namespace BlazorWeb.Wasm;

internal static class HostingExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
    {
        //replaced by built in  .AddAuthenticationStateSerialization()
        //services.AddScoped<AuthenticationStateProvider, PersistingRevalidatingAuthenticationStateProvider>();
        
        services.AddRazorComponents()
            .AddAuthenticationStateSerialization()
            .AddInteractiveServerComponents()
            .AddInteractiveWebAssemblyComponents();

        services.AddCascadingAuthenticationState();
        
        services.AddOidcConfig(configuration, environment);
        
        // ConfigureCookieOidcRefresh attaches a cookie OnValidatePrincipal callback to get
        // a new access token when the current one expires, and reissue a cookie with the
        // new access token saved inside. If the refresh fails, the user will be signed
        // out. OIDC connect options are set for saving tokens and the offline access
        // scope.
        services.ConfigureCookieOidcRefresh(CookieAuthenticationDefaults.AuthenticationScheme, OpenIdConnectDefaults.AuthenticationScheme);


        services.AddSingleton<IAuthorizationMiddlewareResultHandler, BlazorAuthorizationMiddlewareResultHandler>();
        services.AddScoped<HostingEnvironmentService>();
        services.AddSingleton<BaseUrlProvider>();
        
        services.AddHttpContextAccessor();
        services
            .AddTransient<CookieHandler>()
            .AddScoped(sp => sp
                .GetRequiredService<IHttpClientFactory>()
                .CreateClient("API"))
            .AddHttpClient("API", (provider, client) =>
            {
                // Get base address
                var uri = provider.GetRequiredService<BaseUrlProvider>().BaseUrl;
                client.BaseAddress = new Uri(uri);
            }).AddHttpMessageHandler<CookieHandler>();
        
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

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseStaticFiles();
        app.UseAntiforgery();

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