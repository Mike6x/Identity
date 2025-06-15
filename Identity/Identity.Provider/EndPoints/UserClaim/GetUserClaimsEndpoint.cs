using BuildingBlocks.Identity.Users.Abstractions;
using Identity.Core.Features.User;

namespace Identity.Provider.EndPoints.UserClaim;

public static class GetUserClaimsEndpoint
{
    internal static RouteHandlerBuilder MapGetUserClaimsEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapGet("/{userId}/claims", (string userId, IUserService service) =>
            {
                return service.GetUserClaimsAsync(userId, CancellationToken.None);
            })
            .WithName(nameof(GetUserClaimsEndpoint))
            .WithSummary("get user claims")
            // .RequirePermission("Permissions.Handlers.View")
            .WithDescription("get all claims of a user");
    }
}