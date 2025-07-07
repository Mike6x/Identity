using BuildingBlocks.Identity.Users.Abstractions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Identity.Infrastructure.Endpoints.UserClaim;

public static class GetUserClaimsEndpoint
{
    internal static RouteHandlerBuilder MapGetUserClaimsEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapGet("/{userId}/claims", (string userId, IUserService service) 
                => service.GetUserClaimsAsync(userId, CancellationToken.None))
            .WithName(nameof(GetUserClaimsEndpoint))
            .WithSummary("get user claims")
            // .RequirePermission("Permissions.Handlers.View")
            .WithDescription("get all claims of a user");
    }
}