using Identity.Core.Features.Claim.Add;
using Identity.Core.Features.Role;

namespace Identity.Provider.EndPoints.Claim;

public static class AddClaimToRoleEndpoint
{
    internal static RouteHandlerBuilder MapAddClaimToRoleEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPost("/{roleId}/claim", async (
                string roleId,
                AddClaimCommand request,
                IRoleService service,
                CancellationToken cancellationToken) =>
            {
                if (roleId != request.Owner) return Results.BadRequest();
                
                var message = await service.AddClaimToRoleAsync(roleId, request, cancellationToken);
                return Results.Ok(message);
            })
            .WithName(nameof(AddClaimToRoleEndpoint))
            .WithSummary("Add a claim to the Role")
            .WithDescription("Add a claim to the Role");
    }

}
