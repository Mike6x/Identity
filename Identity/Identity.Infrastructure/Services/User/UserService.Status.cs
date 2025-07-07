using BuildingBlocks.Exceptions;
using Identity.Core.Features.User.ToggleUserStatus;
using Identity.Shared.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.Services.User;

public partial class UserService
{
    
    public async Task<bool> ChangeActiveStatusAsync(ToggleUserStatusCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.Users.Where(u => u.Id.ToString() == request.UserId).FirstOrDefaultAsync(cancellationToken)
                   ?? throw new NotFoundException($"User With ID: {request.UserId} Not Found.");

        var isAdmin = await userManager.IsInRoleAsync(user, AppRoles.Admin);
        if (isAdmin)
        {
            throw new GeneralException("Administrators Profile's Status cannot be changed");
        }

        user.IsActive = request.IsActive;

        var result = await userManager.UpdateAsync(user);
        if (result.Succeeded) return result.Succeeded;
        
        var errors = result.Errors.Select(error => error.Description).ToList();
        throw new GeneralException("Changing user active  status operation failed", errors);

    }
    
    public async Task<bool> ChangeOnlineStatusAsync(string userId, bool isOnline, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(userId) ?? throw new NotFoundException($"User With ID: {userId} Not Found.");

        user.IsOnline = isOnline;

        var result = await userManager.UpdateAsync(user);
        return result.Succeeded;
    }
    
    
    public async Task <bool> LockUserAsync(string userId, int lockedDays, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(userId)
                   ?? throw new NotFoundException($"User with Id: {userId} doesn't exist.");
        
        IdentityResult result;
        if (lockedDays > 0)
        {
            result = await userManager.SetLockoutEnabledAsync(user, true);
            await userManager.SetLockoutEndDateAsync(user, DateTimeOffset.Now.AddDays(90));
            await userManager.UpdateSecurityStampAsync(user);
        }
        else
        {
            result = await userManager.SetLockoutEndDateAsync(user, null);
        }
        
        return result.Succeeded;
    }
}