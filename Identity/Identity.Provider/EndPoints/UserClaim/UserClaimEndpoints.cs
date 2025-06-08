namespace Identity.Provider.EndPoints.UserClaim;

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