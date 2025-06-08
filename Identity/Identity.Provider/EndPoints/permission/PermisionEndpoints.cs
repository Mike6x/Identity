namespace Identity.Provider.EndPoints.permission;

public static class PermisionEndpoints
{
    public static IEndpointRouteBuilder MapRolePermissionEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGetRolePermissionsEndpoint();
        app.MapUpdateRolePermissionsEndpoint();
        
        return app;
    }
}