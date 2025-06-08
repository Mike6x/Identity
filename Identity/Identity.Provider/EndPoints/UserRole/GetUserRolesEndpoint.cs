using Identity.Core.Features.User;

namespace Identity.Provider.EndPoints.UserRole;
public static class GetUserRolesEndpoint
{
    internal static RouteHandlerBuilder MapGetUserRolesEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapGet("/{userId}/roles", (string userId, IUserService service, CancellationToken cancellationToken) 
                => service.GetUserRolesAsync(userId, cancellationToken))
        .WithName(nameof(GetUserRolesEndpoint))
        .WithSummary("get user roles")
        // .RequirePermission("Permissions.Handlers.View")
        .WithDescription("get user roles");
    }
}
