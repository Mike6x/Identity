using BuildingBlocks.Auth.Policy;
using BuildingBlocks.Identity.Users.Abstractions;
using Identity.Core.Features.User.CreateUser;

namespace Identity.Provider.EndPoints.User.BasicFeatures;

public static class CreateUserEndpoint
{
    internal static RouteHandlerBuilder MapCreateUserEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPost("/", (                 
                HttpContext context,
                CreateUserCommand request,
                IUserService service,
                CancellationToken cancellationToken) =>
            {
                var origin = $"{context.Request.Scheme}://{context.Request.Host.Value}{context.Request.PathBase.Value}";
                return service.CreateAsync(request, origin, cancellationToken);
            })
            .WithName(nameof(CreateUserEndpoint))
            .WithSummary("Create user")
            .WithDescription("Create new user")
            .RequirePermission("Permissions.Users.Create")
            //.MapToApiVersion(1)
            ;
    }
}