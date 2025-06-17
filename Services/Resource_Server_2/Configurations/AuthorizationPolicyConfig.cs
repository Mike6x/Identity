using Identity.Shared.Authorization;

namespace Resource_Server_2.Configurations;

public static class AuthorizationPolicyConfig
{
    public static IServiceCollection AddAuthorizationPolicy( this IServiceCollection services)
    {
               
        services.AddAuthorizationBuilder()
            .AddPolicy(AppPolicies.CanManageCities, policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireRole(AppRoles.Superuser);
            });
        services.AddAuthorizationBuilder()
            .AddPolicy(AppPolicies.CanManageStudents, policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireRole(AppRoles.Admin);
            });
        
        
        services.AddAuthorizationBuilder()
            .AddPolicy(AppPolicies.PaidForecast, policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireRole(AppRoles.Admin);
            });
        
        services.AddAuthorizationBuilder()
            .AddPolicy(AppPolicies.WeatherRead, policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireClaim("permission", "weather:read");
                // policy.RequireClaim(ClaimConstants.Permissions, AppScopes.WeatherReadScope);
            });
        
        services.AddAuthorizationBuilder()
            .AddPolicy(AppPolicies.SecureForecast, policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireClaim("read-weather", "true");
            });
        
        return services;
    }
    
}