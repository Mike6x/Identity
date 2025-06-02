using System.Data;
using Identity.Shared.Configurations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace Resource_Server_3.Configurations;

public static class OpenIdAuthConfig
{
    
    public static IServiceCollection AddOpenIdAuth(this IServiceCollection services, IConfiguration configuration, List<string> policyNames)
    {
        //var authOptions = services.BindValidateReturn<OpenIdOptions>(config);
        
        // Get the security configuration constants
        var securityConfig = configuration.GetSection("SecurityConfig").Get<SecurityConfig>() ??
                             throw new NullReferenceException("SecurityConfig is null");

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.Authority = securityConfig.Issuer;
            options.Audience = securityConfig.Audience;
            options.RequireHttpsMetadata = false;
            options.SaveToken = true;
            options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
            {
                RequireAudience = true,
                ValidateAudience = true,
            };
            options.Events = new JwtBearerEvents
            {
                OnChallenge = context =>
                {
                    context.HandleResponse();
                    if (!context.Response.HasStarted)
                    {
                      //  throw new UnauthorizedException(context.Error!, context.ErrorDescription!);
                        throw new UnauthorizedAccessException(context.Error!);
                    }
            
                    return Task.CompletedTask;
                },
                OnForbidden = _ =>  throw new NullReferenceException("SecurityConfig is null")
                    //throw new ForbiddenException()
            };
        });


        {
            services.AddAuthorization(options =>
            {
                foreach (string policyName in policyNames)
                {
                    options.AddPolicy(policyName, policy =>
                    {
                        policy.Requirements.Add(new HasScopeRequirement(policyName, securityConfig.Issuer!));
                    });
                }
            });
        }
        
        services.AddSingleton<IAuthorizationHandler, HasScopeHandler>();
        
        return services;
    }
}
