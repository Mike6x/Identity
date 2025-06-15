using BuildingBlocks.Identity.Users.Abstractions;
using Identity.Core.Features.User;

namespace Identity.Provider.EndPoints.User.Others;
public static class GetUserByPhoneNumberEndpoint
{
    internal static RouteHandlerBuilder MapGetUserByPhoneNumberEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapGet("phoneNumber/{phoneNumber}/", (string phoneNumber, IUserService service) =>
        {
            return service.GetByPhoneAsync(phoneNumber, CancellationToken.None);
        })
        .WithName(nameof(GetUserByPhoneNumberEndpoint))
        .WithSummary("Get user profile by Phone Number")
        // .RequirePermission("Permissions.Handlers.View")
        .WithDescription("Get another user's profile details by phone number.");
    }
}
