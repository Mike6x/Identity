using BuildingBlocks.Exceptions;
using Identity.Core.Features.Claim;
using Identity.Core.Features.Role.UpdatePermissions;
using Identity.Shared.Authorization;

namespace Identity.Infrastructure.Services.Role;
public partial class RoleService
{
   
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
        foreach (var claim in currentClaims.Where(c => !request.Permissions.Exists(p => p == c.Value)))
        {
            var result = await roleManager.RemoveClaimAsync(role, claim);
            if (result.Succeeded) continue;
            var errors = result.Errors.Select(error => error.Description).ToList();
            throw new GeneralException("operation failed", errors);
        }

        // Add all permissions that were not previously selected
        foreach (var permission in request.Permissions.Where(c => currentClaims.All(p => p.Value != c)))
        {
            if (string.IsNullOrEmpty(permission)) continue;
            
            var claimDto = new ClaimViewModel
            { 
                 Enabled = true,
                 Type = AppClaims.Permission,
                 Value = permission,
                 IncludeInAccessToken = true,
                 IncludeInIdentityToken = true
            };
            
            await roleManager.AddClaimAsync(role, claimDto.ToClaim());
        }

        return "permissions updated";
    }
    
}