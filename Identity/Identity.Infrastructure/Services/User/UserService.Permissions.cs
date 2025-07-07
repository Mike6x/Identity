using BuildingBlocks.Caching;
using BuildingBlocks.Exceptions;
using Identity.Core.Entities;
using Identity.Shared.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.Services.User;

public sealed partial class UserService
{
    public async Task<List<string>?> GetPermissionsAsync(string userId, CancellationToken cancellationToken)
    {
        var permissions = await CacheServiceExtensions.GetOrSetAsync(cache, GetPermissionCacheKey(userId),
            async () =>
            {
                var user = await userManager.FindByIdAsync(userId) ?? throw new UnauthorizedException();

                var userRoles = await userManager.GetRolesAsync(user);
                var permissions = new List<string>();
                foreach (var role in await Queryable
                    .Where<AppRole>(roleManager.Roles, r => userRoles.Contains(r.Name!))
                    .ToListAsync(cancellationToken))
                {
                    permissions.AddRange(await Queryable
                        .Where<IdentityRoleClaim>(dbContext.RoleClaims, rc => rc.RoleId == role.Id && rc.ClaimType == AppClaims.Permission)
                        .Select(rc => rc.ClaimValue!)
                        .ToListAsync(cancellationToken));
                }
                return permissions.Distinct().ToList();
            },
            cancellationToken: cancellationToken);

        return permissions;
    }
    private static string GetPermissionCacheKey(string userId)
    {
        return $"perm:{userId}";
    }

    public async Task<bool> HasPermissionAsync(string userId, string permission, CancellationToken cancellationToken = default)
    {
        var permissions = await GetPermissionsAsync(userId, cancellationToken);

        return permissions?.Contains(permission) ?? false;
    }

    public Task InvalidatePermissionCacheAsync(string userId, CancellationToken cancellationToken)
    {
        return cache.RemoveAsync(GetPermissionCacheKey(userId), cancellationToken);
    }

    public async Task<List<string>?> GetUserPermissionsAsync(string userId, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(userId) ?? throw new NotFoundException("user not found");

        var userRoles = await userManager.GetRolesAsync(user);
        var permissions = new List<string>();
        foreach (var role in await Queryable
                     .Where<AppRole>(roleManager.Roles, r => userRoles.Contains(r.Name!))
                     .ToListAsync(cancellationToken))
        {
            permissions.AddRange(await Queryable
                .Where<IdentityRoleClaim>(dbContext.RoleClaims, rc => rc.RoleId == role.Id && rc.ClaimType == AppClaims.Permission)
                .Select(rc => rc.ClaimValue!)
                .ToListAsync(cancellationToken));
        }
        return permissions.Distinct().ToList();
    }
}
