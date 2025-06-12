using Identity.Shared.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Client.Infrastructure.Auth;
public static class RegisterAuthServices
{
    public static IServiceCollection AddAuthentication(this IServiceCollection services, IConfiguration config)
    {
        services.AddAuthorizationCore(RegisterPermissionClaims);

        return services;
    }


    private static void RegisterPermissionClaims(AuthorizationOptions options)
    {
        foreach (var permission in AppPermissions.All)
        {
            options.AddPolicy(permission.Name, policy => policy.RequireClaim(AppClaims.Permission, permission.Name));
        }
    }
}
