using BuildingBlocks.Auth.Policy;
using BuildingBlocks.Identity.Users.Abstractions;

namespace Identity.Provider.EndPoints.User.BasicFeatures;
public static class DeleteUserEndpoint
{
    internal static RouteHandlerBuilder MapDeleteUserEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapDelete("/{userId:guid}", (string userId, IUserService service) 
                => service.DeleteAsync(userId))
        .WithName(nameof(DeleteUserEndpoint))
        .WithSummary("delete a user")
        .WithDescription("delete a user")
        .RequirePermission("Permissions.Users.Remove");
    }
}
