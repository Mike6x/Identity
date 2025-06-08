using Identity.Core.Features.Claim.Remove;
using Identity.Core.Features.Role;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Provider.EndPoints.Claim;

public static class RemoveClaimOfRoleEndpoint
{
    internal static RouteHandlerBuilder MapRemoveClaimOfRoleEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapDelete("/{roleId}/claim", async (
                string roleId,
                [FromBody]RemoveClaimCommand command,
                IRoleService service,
                CancellationToken cancellationToken) =>
            {
                if (roleId != command.Owner) return Results.BadRequest();
                var message = await service.RemoveClaimOfRoleAsync(roleId, command, cancellationToken);

                return Results.Ok(message);
            })
            .WithName(nameof(RemoveClaimOfRoleEndpoint))
            .WithSummary("Remove a claim from Role ")
            .WithDescription("Remove a claim from Role ");
    }
}