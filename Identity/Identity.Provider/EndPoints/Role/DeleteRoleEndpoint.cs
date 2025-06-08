using Identity.Core.Features.Role;

namespace Identity.Provider.EndPoints.Role;

public static class DeleteRoleEndpoint
{
    public static RouteHandlerBuilder MapDeleteRoleEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapDelete("/{roleId:guid}", async (string roleId, IRoleService service) =>
        {
            await service.DeleteAsync(roleId);
        })
        .WithName(nameof(DeleteRoleEndpoint))
        .WithSummary("Remove a role by ID")
        // .RequirePermission("Permissions.Handlers.Remove")
        .WithDescription("Remove a role from the system by its ID.");
    }
}

