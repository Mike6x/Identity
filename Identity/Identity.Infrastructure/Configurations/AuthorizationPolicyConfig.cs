using BuildingBlocks.Auth.Policy;
using Identity.Shared.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Infrastructure.Configurations;

public static class AuthorizationPolicyConfig
{
    public static IServiceCollection AddAuthorizationPolicy( this IServiceCollection services)
    {
        services.AddAuthorizationBuilder()
            // .AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"))
            // .AddPolicy("UserOnly", policy => policy.RequireRole("Basic"));
            .AddPolicy(AppPolicies.CanManageApplications, policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireClaim(ClaimConstants.ReadWriteClaim, "applications");
            })
            .AddPolicy(AppPolicies.CanManageScopes, policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireClaim(ClaimConstants.ReadWriteClaim, "scopes");
            })
            .AddPolicy(AppPolicies.CanManageUsers, policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireClaim(ClaimConstants.ReadWriteClaim, "users");
            })
            .AddPolicy(AppPolicies.CanManageRoles, policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireClaim(ClaimConstants.ReadWriteClaim, "roles");
            });
    
        
        // services.AddScoped<IAuthorizationHandler, RequiredPermissionAuthorizationHandler>();
        services.AddAuthorizationBuilder().AddRequiredPermissionPolicy();
        
        // services.AddAuthorization(options =>
        // {
        //     options.FallbackPolicy = options.GetPolicy(RequiredPermissionDefaults.PolicyName);
        // });
        
        return services;
    }
    
}