using BuildingBlocks.Identity.Users.Abstractions;
using Identity.Core.Features.User;

namespace Identity.Provider.EndPoints.User.BasicFeatures;
public static class GetUsersEndpoint
{
    internal static RouteHandlerBuilder MapGetUsersEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapGet("/", (CancellationToken cancellationToken, IUserService service) =>
        {
            return service.GetAllAsync(cancellationToken);
        })
        .WithName(nameof(GetUsersEndpoint))
        .WithSummary("get all users ")
        .WithDescription("get all users ")
        // .RequirePermission("Permissions.Users.Search")
        ;
    }
}
