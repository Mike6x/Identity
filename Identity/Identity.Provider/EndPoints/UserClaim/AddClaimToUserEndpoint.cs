using BuildingBlocks.Identity.Users.Abstractions;
using Identity.Core.Features.Claim.Add;
using Identity.Core.Features.User;

namespace Identity.Provider.EndPoints.UserClaim;

public static class AddClaimToUserEndpoint
{
    internal static RouteHandlerBuilder MapAddClaimToUserEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPost("/{userId}/claim", async (
                HttpContext context,
                string userId,
                AddClaimCommand command,
                IUserService service,
                CancellationToken cancellationToken) =>
            {
                if (userId != command.Owner) return Results.BadRequest();
                
                var message = await service.AddClaimToUserAsync(userId, command, cancellationToken);
                return Results.Ok(message);
            })
            .WithName(nameof(AddClaimToUserEndpoint))
            .WithSummary("Add a claim to User")
            .WithDescription("Add a claim to User");
    }

}
