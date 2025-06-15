using Identity.Shared.Authorization;

namespace Identity.Admin.Configurations;

public static class AuthorizationPolicyConfig
{
    public static IServiceCollection AddAuthorizationPolicy( this IServiceCollection services)
    {
        services.AddAuthorizationCore(options =>
        {
            // options.AddPolicy(AppScopes.UserReadScope, policy => 
            //     policy.RequireClaim(ClaimConstants.Permissions, AppScopes.UserReadScope));
            //
            options.AddPolicy(AppScopes.WeatherReadScope, policy => 
                policy.RequireRole("Admin"));
            
            options.AddPolicy(AppPolicies.CanManageApplications, policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireClaim(ClaimConstants.ReadWriteClaim, "applications");
            });
            options.AddPolicy(AppPolicies.CanManageScopes, policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireClaim(ClaimConstants.ReadWriteClaim, "scopes");
            });
            options.AddPolicy(AppPolicies.CanManageUsers, policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireClaim(ClaimConstants.ReadWriteClaim, "users");
            });
            options.AddPolicy(AppPolicies.CanManageRoles, policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireClaim(ClaimConstants.ReadWriteClaim, "roles");
            });
        });
        
        return services;
    }
    
}