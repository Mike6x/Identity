using BuildingBlocks.Auth.Policy;
using BuildingBlocks.Identity.Users.Abstractions;
using Identity.Core.Features.User;
using Identity.Shared.Authorization;

namespace Identity.Provider.EndPoints.User.BasicFeatures;
public static class GetUserEndpoint
{
    internal static RouteHandlerBuilder MapGetUserEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapGet("/{userId:guid}", (string userId, IUserService service) =>
        {
            return service.GetAsync(userId, CancellationToken.None);
        })
        .WithName(nameof(GetUserEndpoint))
        .WithSummary("Get user profile by ID")
        .RequirePermission("Permissions.Users.View")
  
        .WithDescription("Get another user's profile details by user ID.");
    }
}
