using BuildingBlocks.Identity.Users.Abstractions;
using Identity.Core.Features.User;
using Identity.Shared.Authorization;

namespace Identity.Provider.EndPoints.User.Others;
public static class GetUserByEmailEndpoint
{
    internal static RouteHandlerBuilder MapGetUserByEmailEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapGet("/email/{email}", (string email, IUserService service) =>
        {
            return service.GetByEmailAsync(email, CancellationToken.None);
        })
        .WithName(nameof(GetUserByEmailEndpoint))
        .WithSummary("Get user profile by Name")
        .RequireAuthorization(AppPolicies.CanManageUsers)
        // .RequirePermission("Permissions.Handlers.View")
        .WithDescription("Get another user's profile details by userName.");
    }
}
