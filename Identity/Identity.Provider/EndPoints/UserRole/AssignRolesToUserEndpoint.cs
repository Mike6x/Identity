using BuildingBlocks.Identity.Users.Abstractions;
using Identity.Core.Features.User;
using Identity.Core.Features.User.AssignUserRole;

namespace Identity.Provider.EndPoints.UserRole;
public static class AssignRolesToUserEndpoint
{
    internal static RouteHandlerBuilder MapAssignRolesToUserEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPost("/{userId}/roles", async (AssignUserRoleCommand command,
            string userId,
            IUserService userService,
            CancellationToken cancellationToken) =>
        {

            var message = await userService.AssignRolesToUserAsync(userId, command, cancellationToken);
            return Results.Ok(message);
        })
        .WithName(nameof(AssignRolesToUserEndpoint))
        .WithSummary("assign roles to a user")
        .WithDescription("assign roles to a user");
    }

}
