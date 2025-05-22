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
        services.AddSecurityHeaderPolicies()
            .SetPolicySelector((PolicySelectorContext ctx) =>
            {
                return SecurityHeadersDefinitions.GetHeaderPolicyCollection(environment.IsDevelopment(),
                    configuration["OIDCSettings:Authority"]);
            });
        
        services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
            .AddCookie()
            .AddOpenIdConnect(options =>
            {
                configuration.GetSection("OIDCSettings").Bind(options);

                options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.ResponseType = OpenIdConnectResponseType.Code;

                options.SaveTokens = true;
                options.GetClaimsFromUserInfoEndpoint = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = "name"
                };
            });

        return services;
    }

}