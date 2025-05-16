using Microsoft.IdentityModel.Tokens;

namespace Resource_Server_3.Configurations;

public static class OpenIdDictConfig
{
    public static IServiceCollection AddOpenIdDictConfig(this IServiceCollection services, IConfiguration configuration)
    {
        // Get the security configuration constants
        var securityConfig = configuration.GetSection("SecurityConfig").Get<SecurityConfig>() ??
                             throw new NullReferenceException("SecurityConfig is null");
        
        // Add OpenIddict validation
        services.AddOpenIddict()
            .AddValidation(options =>
            {
                options.SetIssuer(securityConfig.Issuer);
                options.AddAudiences(securityConfig.Audience);

                options.AddEncryptionKey(new SymmetricSecurityKey(Convert.FromBase64String(securityConfig.Key)));

                options.UseSystemNetHttp();
                options.UseAspNetCore();
            });
        return services;
    }
}