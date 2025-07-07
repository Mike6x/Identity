using Identity.Core.Features.Role;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Identity.Infrastructure.Endpoints.RoleClaim;

public static class GetRoleClaimsEndpoint
{
    public static RouteHandlerBuilder MapGetRoleClaimsEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapGet("/{roleId}/claims", async (
                string roleId, 
                IRoleService roleService,
                CancellationToken cancellationToken) 
                => await roleService.GetRoleClaimsAsync(roleId, cancellationToken))
                                        .WithName(nameof(GetRoleClaimsEndpoint))
                                        .WithSummary("Get all claims of the role.")
                                        // .RequirePermission("Permissions.Handlers.View")
                                        .WithDescription("Retrieve all claim of a role.");
    }
}
