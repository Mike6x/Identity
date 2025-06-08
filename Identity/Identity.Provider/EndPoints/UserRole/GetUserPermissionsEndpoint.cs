using Identity.Core.Features.User;

namespace Identity.Provider.EndPoints.UserRole;
public static class GetUserPermissionsEndpoint
{
    internal static RouteHandlerBuilder MapGetUserPermissionsEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapGet("/{userId}/permissions", async (
                string userId, 
                IUserService service, 
                CancellationToken cancellationToken) 
                    => await service.GetUserPermissionsAsync(userId, cancellationToken))
                                    .WithName(nameof(GetUserPermissionsEndpoint))
                                    .WithSummary("Get user permissions")
                                    .WithDescription("Get all permissions of a user");
            }
}
