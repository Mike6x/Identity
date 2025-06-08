using Identity.Core.Features.Claim.Update;
using Identity.Core.Features.User;

namespace Identity.Provider.EndPoints.UserClaim;

public static class AssignClaimsToUserEndpoint
{
    internal static RouteHandlerBuilder MapAssignClaimsToUserEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPost("/{userId}/claims", (
                string userId, 
                AssignClaimsCommand request,
                IUserService service,
                CancellationToken cancellationToken) =>
            {
                return service.AssignClaimsToUserAsync(userId, request, cancellationToken);
            })
            .WithName(nameof(AssignClaimsToUserEndpoint))
            .WithSummary("Assign a list of claims")
            // .RequirePermission("Permissions.Handlers.View")
            .WithDescription("Assign a list of claims to a user");
    }
}