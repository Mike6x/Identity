using FluentValidation;
using Identity.Core.Features.Claim.Update;
using Identity.Core.Features.Role;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Provider.EndPoints.Claim;
public static class UpdateClaimsToRoleEndpoint
{
    public static RouteHandlerBuilder MapUpdateClaimsToRoleEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPut("/{roleId}/claims", async (
            string roleId,
            AssignClaimsCommand request,
            IRoleService roleService,
            [FromServices] IValidator<AssignClaimsCommand> validator,
            CancellationToken cancellationToken) =>
        {
            if (roleId != request.Owner) return Results.BadRequest();
            var response = await roleService.UpdateClaimsToRoleAsync(roleId,request,cancellationToken);
            return Results.Ok(response);
        })
        .WithName(nameof(UpdateClaimsToRoleEndpoint))
        .WithSummary("update role Claims")
        // .RequirePermission("Claims.Handlers.Create")
        .WithDescription("Replace all by new list of Claims");
    }
}
