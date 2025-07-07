using BuildingBlocks.Identity.Users.Abstractions;
using Identity.Shared.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Identity.Infrastructure.Endpoints.User.Others;
public static class GetUserByEmailEndpoint
{
    internal static RouteHandlerBuilder MapGetUserByEmailEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapGet("email/{email}", (string email, IUserService service) =>
        {
            return service.GetByEmailAsync(email, CancellationToken.None);
        })
        .WithName(nameof(GetUserByEmailEndpoint))
        .WithSummary("Get user profile by Email")
        // .RequireAuthorization(AppPolicies.CanManageUsers)
        // .RequirePermission("Permissions.Handlers.View")
        .WithDescription("Get another user's profile details by userName.");
    }
}
