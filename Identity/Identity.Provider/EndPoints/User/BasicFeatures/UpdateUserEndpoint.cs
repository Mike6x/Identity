using BuildingBlocks.Identity.Users.Abstractions;
using Identity.Core.Features.User;
using Identity.Core.Features.User.UpdateUser;

namespace Identity.Provider.EndPoints.User.BasicFeatures;
public static class UpdateUserEndpoint
{
    internal static RouteHandlerBuilder MapUpdateUserEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPut("/", ( HttpContext context,
            string id,
            UpdateUserCommand request,   
            IUserService service,
            CancellationToken cancellationToken) =>
        {
            
            var origin = $"{context.Request.Scheme}://{context.Request.Host.Value}{context.Request.PathBase.Value}";

            return service.UpdateProfileAsync(request, id, origin, cancellationToken);
        })
        .WithName(nameof(UpdateUserEndpoint))
        .WithSummary("update user profile")
        .WithDescription("update user profile")
        // .RequirePermission("Permissions.Handlers.Update")
        ;
    }
}
