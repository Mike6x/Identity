using Microsoft.AspNetCore.Routing;

namespace Identity.Infrastructure.Endpoints.Claim;

public static class ClaimEndpoints
{
    public static IEndpointRouteBuilder MapRoleClaimEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapAssignClaimsToRoleEndpoint();
        
        app.MapAddClaimToRoleEndpoint();
        app.MapGetRoleClaimsEndpoint();
      
        app.MapUpdateClaimsToRoleEndpoint();
        app.MapChangeClaimOfRoleEndpoint();
        app.MapRemoveClaimOfRoleEndpoint();

        return app;
    }
}