using Identity.Core.Entities;
using Identity.Infrastructure.Data;
using Identity.Infrastructure.Data.Store;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Identity.Infrastructure.Configurations;
public static class IdentityConfig
{
    public static IServiceCollection AddIdentityConfig(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddIdentity<AppUser, AppRole>(options =>
            {
                options.SignIn.RequireConfirmedAccount = true;
                options.SignIn.RequireConfirmedEmail = true;
                options.SignIn.RequireConfirmedPhoneNumber = false;

                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 1;

                options.User.RequireUniqueEmail = true;
                
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
                options.Lockout.MaxFailedAccessAttempts = 5;

            })
            .AddSignInManager()
            .AddDefaultTokenProviders()
            // .AddRoles<AppRole>() 
            .AddRoleStore<AppRoleStore>()
            .AddUserStore<AppUserStore>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultUI();;
        
        // RegisterUser scopes (permissions)
        services
            .Configure<IdentityOptions>(options =>
            {
                // Configure Identity to use the same JWT claims as OpenIdDict instead
                // of the legacy WS-Federation claims it uses by default (ClaimTypes),
                // which saves you from doing the mapping in your authorization controller.
                //
                // options.ClaimsIdentity.UserNameClaimType = OpenIddictConstants.Claims.Email;
                // => options.ClaimsIdentity.UserNameClaimType = OpenIddictConstants.Claims.Name
                
                options.ClaimsIdentity.UserNameClaimType = Claims.Name;
                options.ClaimsIdentity.UserIdClaimType = Claims.Subject;
                options.ClaimsIdentity.RoleClaimType = Claims.Role;
                options.ClaimsIdentity.EmailClaimType = Claims.Email;
                
                options.SignIn.RequireConfirmedAccount = true;
            }); 
        
        return services;
    }
}
