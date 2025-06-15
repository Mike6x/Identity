using BuildingBlocks.Identity.Users.Abstractions;
using Identity.Core.Features.User;

namespace Identity.Provider.EndPoints.User.ManagementExtensions;

public static class UnLockUserEndpoint
{
    internal static RouteHandlerBuilder MapUnLockUserEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPost("/{userId}/unlock", (string userId, IUserService service, CancellationToken cancellationToken) 
                =>  service.UnlockUserAsync(userId,cancellationToken))
                        .WithName(nameof(UnLockUserEndpoint))
                        .WithSummary("UnLock user")
                        // .RequirePermission("Permissions.Handlers.Remove")
                        .WithDescription("UnLock user");
    }
}