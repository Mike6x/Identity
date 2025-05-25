using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using NetEscapades.AspNetCore.SecurityHeaders.Infrastructure;

namespace WebApp.Mvc.Configurations;

public static class OidcConfig
{
    public static IServiceCollection AddOidcConfig(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
    {
        // services.AddSecurityHeaderPolicies()
        //     .SetPolicySelector((PolicySelectorContext ctx) =>
        //     {
        //         return SecurityHeadersDefinitions.GetHeaderPolicyCollection(environment.IsDevelopment(),
        //             configuration["OIDCSettings:Authority"]);
        //     });
        
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
                options.UsePkce = true;
                
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = "name"
                };
                
                options.SignedOutCallbackPath = "/signout-callback-oidc";
                options.SignedOutRedirectUri = "/Account/PostLogout";
                options.AuthenticationMethod = OpenIdConnectRedirectBehavior.RedirectGet;
                
                                       
                options.Scope.Add("openid");                       
                options.Scope.Add("profile");                      
                options.Scope.Add("roles"); 
                options.Scope.Add("api1"); 
                                                       
                options.ClaimActions.MapJsonKey("role", "role");   
                //options.ClaimActions.MapJsonKey("email","email");
                //options.ClaimActions.MapJsonKey("name", "name"); 
                //options.ClaimActions.MapJsonKey("scope", "scp:api
                //options.ClaimActions.MapJsonKey("scope", "api1");
                
            });

        return services;
    }

}