using Identity.Core.Features.User;

namespace Identity.Provider.EndPoints.User.Others;
public static class GetUserByNameEndpoint
{
    internal static RouteHandlerBuilder MapGetUserByNameEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapGet("/username/{userName}", (string userName, IUserService service) =>
        {
            return service.GetByNameAsync(userName, CancellationToken.None);
        })
        .WithName(nameof(GetUserByNameEndpoint))
        .WithSummary("Get user profile by Name")
        // .RequirePermission("Permissions.Handlers.View")
        .WithDescription("Get another user's profile details by userName.");
    }
}
