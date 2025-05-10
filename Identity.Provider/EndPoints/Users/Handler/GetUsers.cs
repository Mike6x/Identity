using Identity.Core.Dtos.Users;
using Identity.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Identity.Provider.EndPoints.Users.Handler;

public static class GetUsers
{
    public static async Task<List<UserDto>> Handler(UserManager<AppUser> userManager,CancellationToken cancellationToken)
    {
        var users = await userManager.Users.AsNoTracking()
            .Select(user => new UserDto
            { 
                Id = user.Id,
                Email = user.Email ?? "",
                UserName = user.UserName ?? "Anonymous User",
            })
            .ToListAsync(cancellationToken);
        
        return users;
    }
}