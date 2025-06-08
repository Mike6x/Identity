using Identity.Core.Features.User;
using Identity.Core.Features.User.CreateUser;
using Identity.Shared.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Provider.EndPoints.UserAccount;
public static class RegisterUserEndpoint
{
    internal static RouteHandlerBuilder MapRegisterUserEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPost("/register", (CreateUserCommand request,
            [FromHeader(Name = TenantConstants.Identifier)] string tenant,
            IUserService service,
            HttpContext context,
            CancellationToken cancellationToken) =>
        {
            var origin = $"{context.Request.Scheme}://{context.Request.Host.Value}{context.Request.PathBase.Value}";
            return service.CreateAsync(request, origin, cancellationToken);
        })
        .WithName(nameof(RegisterUserEndpoint))
        .WithSummary("self register user")
        .WithDescription("self register user")
        .AllowAnonymous();
    }
}
