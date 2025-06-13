using System.Security.Claims;
using Identity.Core.Entities;
using Identity.Shared.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Infrastructure.Data.Workers;

public partial class OpenIdDictWorker
{
        private static async Task SeedRolesAsync(IServiceScope scope)
    {
                
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<AppRole>>();
        
    //    if (roleManager.Roles.Any()) return;
        
        foreach (var roleName in AppRoles.DefaultRoles)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new AppRole(roleName, $"{roleName} Role for Identity Server"));
            }
            
            var role = await roleManager.FindByNameAsync(roleName);
            
            if (role == null) continue;
            switch (role.Name)
            {
                case AppRoles.Basic:
                    await AssignPermissionsToRoleAsync(roleManager, role, AppPermissions.Basic);
                    break;
                
                case AppRoles.Root:
                    await AssignPermissionsToRoleAsync(roleManager, role, AppPermissions.Root);
                    break;
                
                case AppRoles.Admin:
                    
                    await AssignPermissionsToRoleAsync(roleManager, role, AppPermissions.Admin);
                break;
                
                case AppRoles.Superuser:
                    await AssignAdminDefaultPermissionsToRoleAsync(roleManager, role);
                    await AssignPermissionsToRoleAsync(roleManager, role, AppPermissions.All);
                    break;
            }
        }
    }
        
    private static async Task AssignPermissionsToRoleAsync(
        RoleManager<AppRole> roleManager,
        AppRole role,
        IReadOnlyList<AppPermission> permissions
    )
    {
        var currentClaims = await roleManager.GetClaimsAsync(role);
        var newClaims = permissions
            .Where(permission => !currentClaims.Any(c => c.Type == AppClaims.Permission && c.Value == permission.Name))
            .ToList();
        
        foreach (var claim in newClaims)
        {
            await AddClaimAsync(roleManager, role,AppClaims.Permission, claim.Name);
        }
    }
    
    private static async Task AssignAdminDefaultPermissionsToRoleAsync(
        RoleManager<AppRole> roleManager,
        AppRole role)
    {
        var currentClaims = await roleManager.GetClaimsAsync(role);
        if (!currentClaims.Any())
        {
            await AddClaimAsync(roleManager, role,"identity_read_write", "users");
            await AddClaimAsync(roleManager, role,"identity_read_write", "roles");
            await AddClaimAsync(roleManager, role,"identity_read_write", "applications");
            await AddClaimAsync(roleManager, role,"identity_read_write", "scopes");
        }
    }
    private static async Task AddClaimAsync(RoleManager<AppRole> roleManager, AppRole role, string type, string value)
    {
        var claim = new Claim(type, value);
        claim.Properties.Add("IncludeInAccessToken", "true");
        claim.Properties.Add("IncludeInIdentityToken", "true");
        await roleManager.AddClaimAsync(role, claim);
    }
    
}