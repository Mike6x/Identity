using Identity.API.Models;

namespace Identity.API.Hosting;

public static class CorsConfig
{
    public static IHostApplicationBuilder ConfigureCors(this IHostApplicationBuilder app)
    {
        var configuration = app.Configuration;
        var services = app.Services;

        var origins = configuration.GetSection("Cors:Origins").Get<string[]>() ?? Array.Empty<string>();

        var applications = configuration.GetSection("OpenIddict:ApplicationConfigs").Get<IEnumerable<ApplicationConfig>>();
        if(applications?.Any() == true)
        {
            origins = origins.Concat(
                applications.SelectMany(
                    a => a.RedirectUri?.Select(r => GetBaseAddressFromRedirectUri(r)) ?? Enumerable.Empty<string>())
            ).ToArray();
        }

        services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                policy.WithOrigins(origins)
                    .AllowAnyHeader()
                    .AllowCredentials()
                    .AllowAnyMethod();
            });
        });

        return app;
    }

    public static IApplicationBuilder Cors(this WebApplication app)
    {
        app.UseCors();

        return app;
    }

    private static string GetBaseAddressFromRedirectUri(string redirectUri)
    {
        var uri = new Uri(redirectUri);
        return uri.GetLeftPart(UriPartial.Authority);
    }
}
