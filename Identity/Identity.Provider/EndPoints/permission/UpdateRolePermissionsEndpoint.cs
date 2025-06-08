using FluentValidation;
using Identity.Core.Features.Role;
using Identity.Core.Features.Role.UpdatePermissions;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Provider.EndPoints.permission;
public static class UpdateRolePermissionsEndpoint
{
    public static RouteHandlerBuilder MapUpdateRolePermissionsEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPut("/{roleId}/permissions", async (
            UpdatePermissionsCommand request,
            IRoleService roleService,
            string roleId,
            [FromServices] IValidator<UpdatePermissionsCommand> validator) =>
        {
            if (roleId != request.RoleId) return Results.BadRequest();
            var response = await roleService.UpdatePermissionsToRoleAsync(request);
            return Results.Ok(response);
        })
        .WithName(nameof(UpdateRolePermissionsEndpoint))
        .WithSummary("update role permissions")
        // .RequirePermission("Permissions.Handlers.Create")
        .WithDescription("update role permissions");
    }
}
