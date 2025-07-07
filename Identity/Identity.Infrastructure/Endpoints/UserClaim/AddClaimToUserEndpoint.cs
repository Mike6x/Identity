using BuildingBlocks.Identity.Users.Abstractions;
using Identity.Core.Features.Claim.Add;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Identity.Infrastructure.Endpoints.UserClaim;

public static class AddClaimToUserEndpoint
{
    internal static RouteHandlerBuilder MapAddClaimToUserEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPost("/{userId}/claim", async (
                string userId, 
                AddClaimCommand command,
                IUserService service,
                CancellationToken cancellationToken) 
                => await service.AddClaimToUserAsync(command, cancellationToken))
            .WithName(nameof(AddClaimToUserEndpoint))
            .WithSummary("Add a claim to User")
            .WithDescription("Add a claim to User");
    }

}
