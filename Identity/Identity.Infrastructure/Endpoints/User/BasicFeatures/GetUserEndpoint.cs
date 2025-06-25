using BuildingBlocks.Auth.Policy;
using BuildingBlocks.Identity.Users.Abstractions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Identity.Infrastructure.Endpoints.User.BasicFeatures;
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
