using Identity.Core.Features.Claim.Change;
using Identity.Core.Features.Role;

namespace Identity.Provider.EndPoints.Claim;

public static class ChangeClaimOfRoleEndpoint
{
    internal static RouteHandlerBuilder MapChangeClaimOfRoleEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPut("/{roleId}/claim", async (
                string roleId,
                ChangeClaimCommand command,
                IRoleService service,
                CancellationToken cancellationToken) =>
            {
                if (roleId != command.Owner) return Results.BadRequest();
                
                var message = await service.ChangeClaimOfRoleAsync(roleId, command, cancellationToken);
                return Results.Ok(message);
            })
            .WithName(nameof(ChangeClaimOfRoleEndpoint))
            .WithSummary("Change a Role claim to new")
            .WithDescription("Change a Role claim to new");
    }

}
