using Microsoft.Identity.Web.UI;
using WebApp.Razor.Configurations;

namespace WebApp.Razor;

internal static class HostingExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
    {

        // services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
        //     .AddMicrosoftIdentityWebApp(configuration.GetSection("AzureAdB2C"));
        
        services.AddOidcConfig(configuration, environment);
        
        services.AddRazorPages()
            .AddMicrosoftIdentityUI();
        
        return services;
    }

    public static WebApplication UsePipeline(this WebApplication app)
    {
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();

        app.UseRouting();

        app.UseAuthorization();

        app.MapStaticAssets();
        app.MapRazorPages()
            .WithStaticAssets();
        app.MapControllers();
        
        return app;
    }
    
}