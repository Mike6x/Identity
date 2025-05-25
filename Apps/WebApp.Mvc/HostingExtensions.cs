using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using WebApp.Mvc.Configurations;

namespace WebApp.Mvc;

internal static class HostingExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
    {
        // services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
        //     .AddMicrosoftIdentityWebApp(configuration.GetSection("AzureAdB2C"));
        
        services.AddOidcConfig(configuration, environment);

        services.AddControllersWithViews();
        services.AddRazorPages()
            .AddMicrosoftIdentityUI();
        
        services.AddHttpClient();
        
        return services;
    }

    public static WebApplication UsePipeline(this WebApplication app)
    {
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseRouting();

        app.UseAuthorization();

        app.MapStaticAssets();

        app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
            .WithStaticAssets();

        app.MapRazorPages()
            .WithStaticAssets();
        
        return app;
    }
    
}