using Identity.Core.Features.User;

namespace Identity.Provider.EndPoints.User.Others;
public static class GetUserByEmailEndpoint
{
    internal static RouteHandlerBuilder MapGetUserByEmailEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapGet("/email/{email}", (string email, IUserService service) =>
        {
            return service.GetByEmailAsync(email, CancellationToken.None);
        })
        .WithName(nameof(GetUserByEmailEndpoint))
        .WithSummary("Get user profile by Name")
        // .RequirePermission("Permissions.Handlers.View")
        .WithDescription("Get another user's profile details by userName.");
    }
}
