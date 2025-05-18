using Blazor.Server.Oidc.Configurations;
using Blazor.Server.Oidc.MockData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.IdentityModel.JsonWebTokens;

namespace Blazor.Server.Oidc;

internal static class HostingExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
    {
        
        services.AddOidcConfig(configuration , environment);
        
        services.AddRazorPages().AddMvcOptions(options =>
        {
            var policy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();
            options.Filters.Add(new AuthorizeFilter(policy));
        });
       
        services.AddServerSideBlazor();

        services.AddControllersWithViews(options =>
            options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute()));   
        
        services.AddSingleton<WeatherForecastService>();
        
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

        app.UseSecurityHeaders();

        app.UseHttpsRedirection();

        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapRazorPages();
        app.MapControllers();

        app.MapBlazorHub().RequireAuthorization();
        app.MapFallbackToPage("/_Host");
        
        return app;
    }
}