using Identity.Shared.Authorization;

namespace Resource_Server_3.Configurations;

public static class AuthorizationPolicyConfig
{
    public static IServiceCollection AddAuthorizationPolicy( this IServiceCollection services)
    {
               
        services.AddAuthorizationCore(options =>
        {
            options.AddPolicy(AppPolicies.CanManageStudents, policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireRole(AppRoles.Manager);
            });
            options.AddPolicy(AppPolicies.CanManageCities, policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireRole(AppRoles.Manager);
            });
            
            options.AddPolicy(AppPolicies.PaidForecast, policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireRole(AppRoles.Admin);
            });
            
            options.AddPolicy(AppPolicies.WeatherRead, policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireClaim(ClaimConstants.Permissions, AppScopes.WeatherReadScope);
            });
            
            options.AddPolicy(AppPolicies.SecureForecast, policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireClaim("read-weather", "true");
            });
        });

        return services;
    }
    
}