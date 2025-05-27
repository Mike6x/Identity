using BlazorWeb.Server.Components;
using BlazorWeb.Server.Configurations;
using Client.Infrastructure.Services.MockData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Server.Circuits;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.JsonWebTokens;

namespace BlazorWeb.Server;

internal static class HostingExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
    {
        services.AddOidcConfig(configuration , environment);

        services.TryAddEnumerable(ServiceDescriptor.Scoped<CircuitHandler, BlazorNonceService>(sp =>
            sp.GetRequiredService<BlazorNonceService>()));

        services.AddScoped<BlazorNonceService>();
        
        services.AddCascadingAuthenticationState();
        
        services.AddRazorComponents()
            .AddInteractiveServerComponents();
        
        services.AddRazorPages().AddMvcOptions(options =>
        {
            var policy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();
            options.Filters.Add(new AuthorizeFilter(policy));
        });
        
        services.AddRazorPages().WithRazorPagesRoot("/Components/Pages");
        
        services.AddControllersWithViews(options =>
            options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute()));
        
        services.AddSingleton<LocalWeatherForecastService>();
        
        services.RegisterHttpClient(configuration);
        
        return services;
    }

    public static WebApplication UsePipeline(this WebApplication app)
    {
        JsonWebTokenHandler.DefaultInboundClaimTypeMap.Clear();

        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }

        // Using an unsecure CSP as CSP nonce is not supported in Blazor Web ...
        app.UseSecurityHeaders();

        app.UseMiddleware<NonceMiddleware>();

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseAntiforgery();

        app.MapRazorPages();
        app.MapControllers();

        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode().RequireAuthorization();
        
        return app;
    }
    
}