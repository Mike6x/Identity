using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using NetEscapades.AspNetCore.SecurityHeaders.Infrastructure;

namespace BlazorWeb.Server.Configurations;

public static class OidcConfig
{
    public static IServiceCollection AddOidcConfig(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
    {
        services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
                options.DefaultSignOutScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
            .AddCookie(options =>
            {
                options.Cookie.Name = "__Host-blazorweb.server";
                options.Cookie.SameSite = SameSiteMode.Lax;
                // can be strict if same-site
                //options.Cookie.SameSite = SameSiteMode.Strict;
            })
            .AddOpenIdConnect(options =>
            {
                configuration.GetSection("OIDCSettings").Bind(options);

                options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.ResponseType = OpenIdConnectResponseType.Code;

                options.SaveTokens = true;
                options.GetClaimsFromUserInfoEndpoint = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = "name",
                    RoleClaimType = "roles"
                };
                options.Scope.Add("api2");
                options.Scope.Add("api3");
            });
        
            services.AddAntiforgery(options =>
            {
                options.HeaderName = "X-XSRF-TOKEN";
                options.Cookie.Name = "__Host-core-X-XSRF-TOKEN";
                options.Cookie.SameSite = SameSiteMode.Strict;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            });
            
            services.AddSecurityHeaderPolicies()
                .SetDefaultPolicy(SecurityHeadersDefinitions
                    .GetHeaderPolicyCollection(configuration["OIDCSettings:Authority"],
                        environment.IsDevelopment()));

        return services;
    }

}