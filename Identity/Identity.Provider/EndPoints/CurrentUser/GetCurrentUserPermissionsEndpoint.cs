using System.Security.Claims;
using BuildingBlocks.Exceptions;
using Identity.Core.Features.User;
using Identity.Shared.Authorization;

namespace Identity.Provider.EndPoints.CurrentUser;
public static class GetCurrentUserPermissionsEndpoint
{
    internal static RouteHandlerBuilder MapGetCurrentUserPermissionsEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapGet("/permissions", async (ClaimsPrincipal user, IUserService service, CancellationToken cancellationToken) =>
        {
            if (user.GetUserId() is not { } userId || string.IsNullOrEmpty(userId))
            {
                throw new UnauthorizedException();
            }

            return await service.GetPermissionsAsync(userId, cancellationToken);
        })
        .WithName(nameof(GetCurrentUserPermissionsEndpoint))
        .WithSummary("Get current user permissions")
        .WithDescription("Get current user permissions");
    }
}
