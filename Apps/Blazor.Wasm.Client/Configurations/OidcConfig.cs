using Blazor.Wasm.Client.Security;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace Blazor.Wasm.Client.Configurations;

public static class OidcConfig
{
    public static IServiceCollection AddOpenIdDictCfg(this IServiceCollection services, IConfiguration configuration)
    {
                        
        services.AddOidcAuthentication(options =>
            {
                configuration.Bind("OpenIDConnectSettings", options.ProviderOptions);
                options.ProviderOptions.ResponseType = "code";
                options.ProviderOptions.ResponseMode = "query";
                options.AuthenticationPaths.RemoteRegisterPath = "./api/users/register";
            
                options.UserOptions.RoleClaim = "role";
                
                options.ProviderOptions.DefaultScopes.Clear();
                options.ProviderOptions.DefaultScopes.Add("openid");
                options.ProviderOptions.DefaultScopes.Add("roles");
                options.ProviderOptions.DefaultScopes.Add("api1");
            
                // options.ProviderOptions.DefaultScopes.Add("service-api-scope");
            
            })
            .AddAccountClaimsPrincipalFactory<IdentityClaimsPrincipalFactory<RemoteUserAccount>>();
       
        services.AddAuthorizationCore(options =>
        {
            options.AddPolicy("ReadWeatherDataPolicy", policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireClaim("read-weather", "true");
            });    
        });
        
        return services;
    }
    

    public static IServiceCollection AddOpenIdDictConfig(this IServiceCollection services, string authorityUrl)
    {
                
        services.AddOidcAuthentication(options =>
        {
            options.ProviderOptions.ClientId = "web-client";
            options.ProviderOptions.Authority = authorityUrl;
            options.ProviderOptions.ResponseType = "code";
        
            options.ProviderOptions.ResponseMode = "query";
            options.AuthenticationPaths.RemoteRegisterPath = "./api/users/register";
            
            options.UserOptions.RoleClaim = "role";
            
            options.ProviderOptions.DefaultScopes.Clear();
            options.ProviderOptions.DefaultScopes.Add("openid");
            options.ProviderOptions.DefaultScopes.Add("roles");
            options.ProviderOptions.DefaultScopes.Add("api1");
            
            // options.ProviderOptions.DefaultScopes.Add("service-api-scope");
            
        })
            .AddAccountClaimsPrincipalFactory<IdentityClaimsPrincipalFactory<RemoteUserAccount>>();
       
        services.AddAuthorizationCore(options =>
        {
            options.AddPolicy("ReadWeatherDataPolicy", policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireClaim("read-weather", "true");
            });    
        });
    
        return services;
    }
    

}