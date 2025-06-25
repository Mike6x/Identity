using Microsoft.AspNetCore.Routing;

namespace Identity.Infrastructure.Endpoints.Role;

public static class RoleEndpoints
{
    public static IEndpointRouteBuilder MapRoleEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapCreateOrUpdateRoleEndpoint();
        app.MapCreateRoleEndpoint();
        app.MapGetRoleEndpoint();
        app.MapGetRolesEndpoint();
        app.MapSearchRolesEndpoint();
        app.MapUpdateRoleEndpoint();
        app.MapDeleteRoleEndpoint();
        
        return app;
    }
}

