using System.Security.Claims;
using BuildingBlocks.Exceptions;
using Identity.Core.Features.Role.UpdatePermissions;
using Identity.Shared.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.Services.Role;

public partial class RoleService
{
    public async Task<List<string>> GetRolePermissionsAsync(string roleId, CancellationToken cancellationToken)
    {
        _ = await GetAsync(roleId) 
            ?? throw new NotFoundException($"Role with Id: {roleId} not found");

        var permissions = await context.RoleClaims
            .Where(c => c.RoleId.ToString() == roleId && c.ClaimType == AppClaims.Permission)
            .Select(c => c.ClaimValue ?? string.Empty)
            .ToListAsync(cancellationToken);

        return permissions;
    }
    
    public async Task<string> UpdatePermissionsToRoleAsync(UpdatePermissionsCommand request)
    {
        var role = await roleManager.FindByIdAsync(request.RoleId) ?? throw new NotFoundException("role not found");
        
        if (role.Name == AppRoles.Admin) throw new ConflictException("operation not permitted");

        // Remove Root Permissions if the Role is not created for Root Tenant.
        // if (multiTenantContextAccessor?.MultiTenantContext?.TenantInfo?.Id != TenantConstants.Root.Id)
        // {
        //     request.Permissions.RemoveAll(u => u.StartsWith("Permissions.Root.", StringComparison.InvariantCultureIgnoreCase));
        // }

        var currentClaims = await roleManager.GetClaimsAsync(role);

        // Remove permissions that were previously selected
        foreach (var claim in Enumerable.Where<Claim>(currentClaims, c => !request.Permissions.Exists(p => p == c.Value)))
        {
            var result = await roleManager.RemoveClaimAsync(role, claim);
            if (!result.Succeeded)
            {
                var errors = Enumerable.Select<IdentityError, string>(result.Errors, error => error.Description).ToList();
                throw new GeneralException("operation failed", errors);
            }
        }

        // Add all permissions that were not previously selected
        foreach (var permission in request.Permissions.Where(c => Enumerable.All<Claim>(currentClaims, p => p.Value != c)))
        {
            if (!string.IsNullOrEmpty(permission))
            {
                context.RoleClaims.Add(new IdentityRoleClaim<>
                {
                    RoleId = role.Id,
                    ClaimType = AppClaims.Permission,
                    ClaimValue = permission,
                    CreatedBy = currentUser.GetUserId(),
                    CreatedAt = DateTime.UtcNow
                });
                await context.SaveChangesAsync();
            }
        }

        return "permissions updated";
    }
    
}