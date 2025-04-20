using Identity.API.Dtos;
using Identity.API.EndPoints.Users.Handler;
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


        group.MapGet("me", CurrentUser.Handler)
            .WithName("CurrentUser")
            .AllowAnonymous();
        
        group.MapPost("/", RegisterUser.Handler)
            .WithName("RegisterUser")
            .AllowAnonymous();

        return app;
    }

}
