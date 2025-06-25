using System.Security.Claims;
using BuildingBlocks.Exceptions;
using BuildingBlocks.Identity.Users.Abstractions;
using Identity.Core.Features.User.UpdateUser;
using Identity.Shared.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Identity.Infrastructure.Endpoints.CurrentUser;
public static class UpdateCurrentUserEndpoint
{
    internal static RouteHandlerBuilder MapUpdateCurrentUserEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPut("/", (
            UpdateUserCommand request, 
            ClaimsPrincipal user, 
            IUserService service, 
            CancellationToken cancellationToken) =>
        {
            if (user.GetUserId() is not { } userId || string.IsNullOrEmpty(userId) )
            {
                throw new UnauthorizedException();
            }
    
            return service.UpdateAsync(request,userId, cancellationToken);
        })
        .WithName(nameof(UpdateCurrentUserEndpoint))
        .WithSummary("update current user profile")
        // .RequirePermission("Permissions.Handlers.Update")
        .WithDescription("Update profile of currently logged in user.");
    }
}
