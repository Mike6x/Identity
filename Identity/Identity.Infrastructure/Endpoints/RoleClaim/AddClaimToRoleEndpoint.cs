using Identity.Core.Features.Claim.Add;
using Identity.Core.Features.Role;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Identity.Infrastructure.Endpoints.RoleClaim;

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
                var response = await service.AddClaimToRoleAsync(request, cancellationToken);
                return response;
            })
            .WithName(nameof(AddClaimToRoleEndpoint))
            .WithSummary("Add a claim to the Role")
            .WithDescription("Add a claim to the Role");
    }

}
