using BuildingBlocks.Exceptions;
using BuildingBlocks.Identity.Users.Dtos;
using Identity.Core.Entities;
using Identity.Core.Features.User.AssignUserRole;
using Identity.Infrastructure.Services.Role;
using Identity.Shared.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.Services.User;

public sealed partial class UserService
{
    public async Task<List<RoleSummaryDto>> GetUserRolesAsync(string userId, CancellationToken cancellationToken)
    {

        var user = await userManager.FindByIdAsync(userId) 
                   ?? throw new NotFoundException("user not found");
        
        var roles= await roleManager.Roles.AsNoTracking()
            .Select(role => role.ToSummaryDto())
            .ToListAsync(cancellationToken);
        
        foreach (var role in roles)
        {
            role.Enabled = await userManager.IsInRoleAsync(user, role.Name);
        }

        return roles;
    }
    

    
    public async Task<bool> AssignRolesToUserAsync(string userId, AssignUserRoleCommand request, CancellationToken cancellationToken)
    {

        var user = await userManager.FindByIdAsync(userId)
                   ?? throw new NotFoundException($"User with Id: {userId} doesn't exist.");

        if (await userManager.IsInRoleAsync(user, AppRoles.Admin)
            && request.UserRoles.Exists(a => a is { Enabled: false, Name: AppRoles.Admin }))
        {
            // Get count of users in Admin Role
            var adminCount = (await userManager.GetUsersInRoleAsync(AppRoles.Admin)).Count;

            // Check if user is not Root Tenant Admin
            // Edge Case : there are chances for other tenants to have users with the same email as that of Root Tenant Admin. Probably can add a check while User Registration
            
            // if (user.Email == TenantConstants.Root.EmailAddress)
            // {
            //     if (multiTenantContextAccessor?.MultiTenantContext?.TenantInfo?.Id == TenantConstants.Root.Id)
            //     {
            //         throw new GeneralException("action not permitted");
            //     }
            // }
            // else 
            
            if (adminCount <= 2)
            {
                throw new GeneralException("tenant should have at least 2 admins.");
            }
        }

        foreach (var userRole in request.UserRoles)
        {
            if (string.IsNullOrEmpty(userRole.Name) || await roleManager.FindByNameAsync(userRole.Name) is null) continue;
            
            switch (userRole.Enabled)
            {
                case true when !await userManager.IsInRoleAsync(user, userRole.Name):
                    await userManager.AddToRoleAsync(user, userRole.Name);
                    break;
                
                case false when await userManager.IsInRoleAsync(user, userRole.Name):
                    await userManager.RemoveFromRoleAsync(user, userRole.Name);
                    break;
            }
        }

        return true;

    }
    
    // public async Task<string> AssignRolesToUserAsync(string userId, AssignUserRoleCommand request, CancellationToken cancellationToken)
    // {
    //
    //     var user = await userManager.FindByIdAsync(userId)
    //                ?? throw new NotFoundException($"User with Id: {userId} doesn't exist.");
    //
    //     if (await userManager.IsInRoleAsync(user, AppRoles.Admin)
    //         && request.UserRoles.Exists(a => a is { Enabled: false, RoleName: AppRoles.Admin }))
    //     {
    //         // Get count of users in Admin Role
    //         var adminCount = (await userManager.GetUsersInRoleAsync(AppRoles.Admin)).Count;
    //
    //         // Check if user is not Root Tenant Admin
    //         // Edge Case : there are chances for other tenants to have users with the same email as that of Root Tenant Admin. Probably can add a check while User Registration
    //         
    //         // if (user.Email == TenantConstants.Root.EmailAddress)
    //         // {
    //         //     if (multiTenantContextAccessor?.MultiTenantContext?.TenantInfo?.Id == TenantConstants.Root.Id)
    //         //     {
    //         //         throw new GeneralException("action not permitted");
    //         //     }
    //         // }
    //         // else 
    //         
    //         if (adminCount <= 2)
    //         {
    //             throw new GeneralException("tenant should have at least 2 admins.");
    //         }
    //     }
    //
    //     foreach (var userRole in request.UserRoles)
    //     {
    //         if (userRole.RoleName == null || await roleManager.FindByNameAsync(userRole.RoleName) is null) continue;
    //         
    //         switch (userRole.Enabled)
    //         {
    //             case true when !await userManager.IsInRoleAsync(user, userRole.RoleName):
    //                 await userManager.AddToRoleAsync(user, userRole.RoleName);
    //                 break;
    //             
    //             case false when await userManager.IsInRoleAsync(user, userRole.RoleName):
    //                 await userManager.RemoveFromRoleAsync(user, userRole.RoleName);
    //                 break;
    //         }
    //     }
    //
    //     return "User Handlers Updated Successfully.";
    //
    // }
    
}
