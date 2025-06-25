using BuildingBlocks.Identity.Users.Abstractions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Identity.Infrastructure.Endpoints.User.ManagementExtensions;

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