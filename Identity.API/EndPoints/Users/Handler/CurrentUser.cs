using Identity.Core.Dtos.Users;
using Identity.Core.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;

namespace Identity.API.EndPoints.Users.Handler;

public static class CurrentUser
{
    
    public static async Task<Results<Ok<UserDto?>, ForbidHttpResult>> Handler(HttpContext httpContext, UserManager<AppUser> userManager)
    {
        if (httpContext.User?.Identity?.IsAuthenticated != true)
            return TypedResults.Ok<UserDto?>(null);

        var user = await userManager.GetUserAsync(httpContext.User);
        if (user is null)
            return TypedResults.Forbid();
        return TypedResults.Ok<UserDto?>(new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            UserName = user.UserName,
        });
    }

    
}