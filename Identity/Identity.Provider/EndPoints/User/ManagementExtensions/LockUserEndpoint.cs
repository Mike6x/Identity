using BuildingBlocks.Identity.Users.Abstractions;
using Identity.Core.Features.User;

namespace Identity.Provider.EndPoints.User.ManagementExtensions;

public static class LockUserEndpoint
{
    internal static RouteHandlerBuilder MapLockUserEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPost("/{userId}/lock", (string userId, IUserService service,CancellationToken cancellationToken) 
                => service.LockUserAsync(userId,cancellationToken))
                    .WithName(nameof(LockUserEndpoint))
                    .WithSummary("Lock user for 30 days")
                    // .RequirePermission("Permissions.Handlers.Remove")
                    .WithDescription("Lock user");
    }
}