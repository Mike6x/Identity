using Identity.Core.Features.Role;

namespace Identity.Provider.EndPoints.permission;
public static class GetRolePermissionsEndpoint
{
    public static RouteHandlerBuilder MapGetRolePermissionsEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapGet("/{roleId}/permissions", async (
                string roleId, 
                IRoleService roleService, 
                CancellationToken cancellationToken) 
                => await roleService.GetRolePermissionsAsync(roleId, cancellationToken))
                                    .WithName(nameof(GetRolePermissionsEndpoint))
                                    .WithSummary("get role permissions")
                                    // .RequirePermission("Permissions.Handlers.View")
                                    .WithDescription("get all permissions of a role");
    }
}
