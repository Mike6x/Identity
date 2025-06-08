using Identity.Core.Features.Role;

namespace Identity.Provider.EndPoints.Role;
public static class GetRoleEndpoint
{
    public static RouteHandlerBuilder MapGetRoleEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapGet("/{roleId:guid}", async (string roleId, IRoleService service) =>
            {
                return await service.GetAsync(roleId);
            })
            .WithName(nameof(GetRoleEndpoint))
            .WithSummary("Get role details without claims and permissions")
            // .RequirePermission("Permissions.Handlers.View")
            .WithDescription("Retrieve the details of a role by its Id.");
    }
}
