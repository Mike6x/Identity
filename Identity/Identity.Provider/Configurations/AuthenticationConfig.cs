using Microsoft.AspNetCore.Authentication.Cookies;

namespace Identity.Provider.Configurations;

public static class AuthenticationConfig
{
     public static IServiceCollection AddAuthenticationConfig(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme; 
                
                //IdentityOIdc
                // options.DefaultSignInScheme = IdentityConstants.ExternalScheme; 
              
                //Identity Fsh
                //options.DefaultScheme = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme; 
                
            })
            .AddCookie(options =>
            {
                options.LoginPath = "/Account/Login";
                options.LogoutPath = "/Account/Logout";
                options.AccessDeniedPath = "/Account/AccessDenied";
                options.SlidingExpiration = true;
                options.ExpireTimeSpan = TimeSpan.FromHours(8);
            });
        
        return services;
    }
}