using BuildingBlocks.Behaviours;
using FluentValidation;
using Identity.Core.Features.Role;
using Identity.Core.Features.Role.UpdatePermissions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Identity.Infrastructure.Endpoints.permission;
public static class UpdateRolePermissionsEndpoint
{
    public static RouteHandlerBuilder MapUpdateRolePermissionsEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPut("/{roleId}/permissions", async (
            UpdatePermissionsCommand request,
            IRoleService roleService,
            string roleId,
            [FromServices] IValidator<UpdatePermissionsCommand> validator
            ) =>
        {
            var validationResult = await request.ValidateRequest(validator);
            if (validationResult is not null) return validationResult;
            
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
