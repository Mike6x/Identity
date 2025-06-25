using Microsoft.AspNetCore.Routing;

namespace Identity.Infrastructure.Endpoints.UserRole;

public static class UserRoleEndpoints
{
    public static IEndpointRouteBuilder MapUserRoleEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapAssignRolesToUserEndpoint();
        app.MapGetUserRolesEndpoint();
        
        app.MapGetUserPermissionsEndpoint();

        return app;
    }
}