using Identity.Core.Entities;
using Identity.Core.Settings;

namespace Identity.Provider.Configurations;

public static class CorsConfig
{
    public static IServiceCollection AddCorsPolicy(this IServiceCollection services, IConfiguration configuration)
    {

        var allowedOrigins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];

        var applications = configuration.GetSection("OpenIddict:ApplicationConfigs").Get<IEnumerable<ApplicationConfig>>();
        if(applications?.Any() == true)
        {
            allowedOrigins = allowedOrigins.Concat(
                applications.SelectMany(
                    a => a.RedirectUri?.Select(r => GetBaseAddressFromRedirectUri(r)) ?? [])
            ).ToArray();
        }

        services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy", policy =>
            {
                policy.WithOrigins(allowedOrigins)
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });

        return services;
    }
    
    public static IApplicationBuilder UseCorsPolicy(this WebApplication app)
    {
        app.UseCors("CorsPolicy");
        return app;
    }
    
    private static string GetBaseAddressFromRedirectUri(string redirectUri)
    {
        var uri = new Uri(redirectUri);
        return uri.GetLeftPart(UriPartial.Authority);
    }
}
