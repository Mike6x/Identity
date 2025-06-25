using Microsoft.AspNetCore.Routing;

namespace Identity.Infrastructure.Endpoints.CurrentUser;

public static class CurrentUserEndpoints
{
    public static IEndpointRouteBuilder MapCurrentUserEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGetCurrentUserEndpoint(); 
        app.MapUpdateCurrentUserEndpoint();
        app.MapGetCurrentUserPermissionsEndpoint();
        app.MapGetMeEndpoint();
        
        return app;
    }
}