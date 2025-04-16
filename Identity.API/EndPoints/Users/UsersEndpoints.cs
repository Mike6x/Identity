using Identity.API.Dtos;
using Identity.API.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;

namespace Identity.API.EndPoints.Users;

public static class UsersEndpoints
{
    public static IEndpointRouteBuilder MapUsersEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("users")
            .WithTags("Users")
            .WithName("Users");


        group.MapGet("me", CurrentUserHandler)
            .WithName("CurrentUser")
            .AllowAnonymous();

        return app;
    }

    private static async Task<Results<Ok<UserDto?>, ForbidHttpResult>> CurrentUserHandler(HttpContext httpContext, UserManager<ApplicationUser> userManager)
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
