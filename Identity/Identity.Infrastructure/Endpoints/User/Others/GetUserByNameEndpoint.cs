using BuildingBlocks.Identity.Users.Abstractions;
using Identity.Shared.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Identity.Infrastructure.Endpoints.User.Others;
public static class GetUserByNameEndpoint
{
    internal static RouteHandlerBuilder MapGetUserByNameEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapGet("/username/{userName}", (string userName, IUserService service) =>
        {
            return service.GetByNameAsync(userName, CancellationToken.None);
        })
        .WithName(nameof(GetUserByNameEndpoint))
        .WithSummary("Get user profile by Name")
        .RequireAuthorization(AppPolicies.CanManageUsers)
        // .RequirePermission("Permissions.Handlers.View")
        .WithDescription("Get another user's profile details by userName.");
    }
}
