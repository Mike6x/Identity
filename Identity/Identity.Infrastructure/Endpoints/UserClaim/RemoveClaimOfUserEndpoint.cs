using BuildingBlocks.Identity.Users.Abstractions;
using Identity.Core.Features.Claim.Remove;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Identity.Infrastructure.Endpoints.UserClaim;

public static class RemoveClaimOfUserEndpoint
{
    internal static RouteHandlerBuilder MapRemoveClaimOfUserEndpoint(this IEndpointRouteBuilder endpoints)
    {
            return endpoints.MapDelete("/{userId}/claim", async (
                        string userId, 
                        [FromBody]RemoveClaimCommand command,
                        IUserService service,
                        CancellationToken cancellationToken) => await service.RemoveClaimOfUserAsync(command, cancellationToken))
            .WithName(nameof(RemoveClaimOfUserEndpoint))
            .WithSummary("Remove a claim from User ")
            .WithDescription("Remove a claim from User ");
    }
}
