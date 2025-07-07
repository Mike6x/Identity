using BuildingBlocks.Identity.Users.Abstractions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Identity.Infrastructure.Endpoints.User.ManagementExtensions;

public static class SetOnlineStatusEndpoint
{
    internal static RouteHandlerBuilder MapSetOnlineStatusEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPost("/{userId}/set-online", (
                string userId, bool isOnline,
                IUserService service, CancellationToken cancellationToken) 
                => service.ChangeOnlineStatusAsync(userId, isOnline, cancellationToken))
            .WithName(nameof(SetOnlineStatusEndpoint))
            // .AllowAnonymous()
            .WithSummary("update online status")
            .WithDescription("update the user online status.");
    }
}
