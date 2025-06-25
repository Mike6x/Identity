using Microsoft.AspNetCore.Routing;

namespace Identity.Infrastructure.Endpoints.permission;

public static class PermisionEndpoints
{
    public static IEndpointRouteBuilder MapRolePermissionEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGetRolePermissionsEndpoint();
        app.MapUpdateRolePermissionsEndpoint();
        
        return app;
    }
}