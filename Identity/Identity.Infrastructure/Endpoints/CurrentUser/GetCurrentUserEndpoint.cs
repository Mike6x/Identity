using System.Security.Claims;
using BuildingBlocks.Exceptions;
using BuildingBlocks.Identity.Users.Abstractions;
using Identity.Shared.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Identity.Infrastructure.Endpoints.CurrentUser;
public static class GetCurrentUserEndpoint
{
    internal static RouteHandlerBuilder MapGetCurrentUserEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapGet("/", async (ClaimsPrincipal user, IUserService service, CancellationToken cancellationToken) =>
        {
            if (user.GetUserId() is not { } userId || string.IsNullOrEmpty(userId))
            {
                throw new UnauthorizedException();
            }

            return await service.GetByIdAsync(userId, cancellationToken);
        })
        .WithName(nameof(GetCurrentUserEndpoint))
        .WithSummary("Get current user information based on token")
        .WithDescription("Get current user information based on token");
    }
}
