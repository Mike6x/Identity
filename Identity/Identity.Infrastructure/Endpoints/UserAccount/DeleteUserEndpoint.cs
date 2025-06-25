using BuildingBlocks.Exceptions;
using BuildingBlocks.Identity.Users.Abstractions;
using Identity.Core.Features.User.DeleteAccount;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Identity.Infrastructure.Endpoints.UserAccount;
public static class DeleteAccountEndpoint
{
    internal static RouteHandlerBuilder MapDeleteAccountEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPost("/Delete", (HttpContext httpContext, 
                DeleteAccountModel request, 
                IUserService service,CancellationToken cancellationToken) =>
                    {
                        if (httpContext.User.Identity?.IsAuthenticated != true)
                        {
                            throw new UnauthorizedException();
                        }
                        return service.DeleteAccountAsync(httpContext, request, cancellationToken);
                    })
                    .WithName(nameof(DeleteAccountEndpoint))
                    .WithSummary("delete a user account")
                    .RequireAuthorization()
                    // .RequirePermission("Permissions.Users.Remove")
                    .WithDescription("delete a user account");
    }
}
