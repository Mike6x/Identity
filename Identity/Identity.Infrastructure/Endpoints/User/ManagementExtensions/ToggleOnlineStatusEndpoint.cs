using System.Security.Claims;
using BuildingBlocks.Exceptions;
using BuildingBlocks.Identity.Users.Abstractions;
using Identity.Shared.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Identity.Infrastructure.Endpoints.User.ManagementExtensions;
public static class ToggleOnlineStatusEndpoint
{
    internal static RouteHandlerBuilder MapToggleOnlineStatusEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPost("/toggle-online", (
            ClaimsPrincipal user, 
            IUserService service, CancellationToken cancellationToken) =>
        {
            if (user.GetUserId() is not { } userId || string.IsNullOrEmpty(userId))
            {
                throw new UnauthorizedException();
            }
            return service.ChangeOnlineStatusAsync(userId, false, cancellationToken);
        })
        .WithName(nameof(ToggleOnlineStatusEndpoint))
        // .AllowAnonymous()
        .WithSummary("update online status")
        .WithDescription("Update profile of currently logged in user.");
    }
}
