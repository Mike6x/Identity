using Identity.Core.Features.Role;

namespace Identity.Provider.EndPoints.Role;

public static class GetRolesEndpoint
{
    public static RouteHandlerBuilder MapGetRolesEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapGet("/", async (IRoleService service, CancellationToken cancellationToken) =>
                await service.GetAllAsync(cancellationToken))
        .WithName(nameof(GetRolesEndpoint))
        .WithSummary("Get a list of all roles")
        // .RequirePermission("Permissions.Handlers.View")
        .WithDescription("Retrieve a list of all roles available in the system.");
    }
}
