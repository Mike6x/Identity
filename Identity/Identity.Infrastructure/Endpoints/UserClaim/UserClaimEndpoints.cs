using Microsoft.AspNetCore.Routing;

namespace Identity.Infrastructure.Endpoints.UserClaim;

public static class UserClaimEndpoints
{
    public static IEndpointRouteBuilder MapUserClaimEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapAssignClaimsToUserEndpoint();
        
        app.MapAddClaimToUserEndpoint();
        app.MapGetUserClaimsEndpoint();
        
        app.MapChangeClaimOfUserEndpoint();
        app.MapRemoveClaimOfUserEndpoint();
        
        return app;
    }
}