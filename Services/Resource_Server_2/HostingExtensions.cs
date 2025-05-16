using Microsoft.IdentityModel.Tokens;
using OpenIddict.Validation.AspNetCore;
using Resource_Server_2.Configurations;
using Resource_Server_2.Endpoints;
using Scalar.AspNetCore;

namespace Resource_Server_2;

internal static class HostingExtensions
{
    // private static IWebHostEnvironment _env;
    
    public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        var securityConfig = configuration.GetSection("SecurityConfig").Get<SecurityConfig>() ??
                             throw new NullReferenceException("SecurityConfig is null");
        
        services.AddOpenApi();

        services.AddOpenIddict()
            .AddValidation(options =>
            {
                options.SetIssuer(securityConfig.Issuer);
                options.AddAudiences(securityConfig.Audience);

                options.AddEncryptionKey(new SymmetricSecurityKey(Convert.FromBase64String(securityConfig.Key)));

                options.UseSystemNetHttp();
                options.UseAspNetCore();
            });

        services.AddAuthentication(OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme);
        services.AddAuthorizationBuilder()
            .AddPolicy(Constants.AuthPolicy,
                policy => policy.RequireRole("Editor"));
        
        return services;
    }

    public static WebApplication UsePipeline(this WebApplication app)
    {
        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            // app.MapScalarApiReference();
            app.MapScalarApiReference(options => options
                .WithPreferredScheme("OAuth2")
                .AddAuthorizationCodeFlow("OAuth2", flow =>
                {
                    flow.ClientId = "mvc-client";
                    flow.ClientSecret = "388D45FA-B36B-4988-BA59-B187D329C205";
                    flow.Pkce = Pkce.Sha256;
                    flow.SelectedScopes = ["profile", "email", "api"];
                }));
        }

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapResourceEndpoints();
        app.UseHttpsRedirection();

        return app;
    }
}