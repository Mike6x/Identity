using Identity.Core.Features.Claim.Remove;
using Identity.Core.Features.User;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Provider.EndPoints.UserClaim;

public static class RemoveClaimOfUserEndpoint
{
    internal static RouteHandlerBuilder MapRemoveClaimOfUserEndpoint(this IEndpointRouteBuilder endpoints)
    {
            return endpoints.MapDelete("/{userId}/claim", async (
                string userId,
                [FromBody]RemoveClaimCommand command,
                IUserService service,
                CancellationToken cancellationToken) =>
            {
                if (userId != command.Owner) return Results.BadRequest();
                var message = await service.RemoveClaimOfUserAsync(userId, command, cancellationToken);

                return Results.Ok(message);
            })
            .WithName(nameof(RemoveClaimOfUserEndpoint))
            .WithSummary("Remove a claim from User ")
            .WithDescription("Remove a claim from User ");
    }
}
