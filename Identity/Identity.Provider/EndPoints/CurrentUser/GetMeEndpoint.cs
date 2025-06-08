using BuildingBlocks.Exceptions;
using Identity.Core.Features.User;

namespace Identity.Provider.EndPoints.CurrentUser;

public static class GetMeEndpoint
{
    internal static RouteHandlerBuilder MapGetMeEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapGet("/me", async (HttpContext httpContext, IUserService service, CancellationToken cancellationToken) =>
            {
                if (httpContext.User?.Identity?.IsAuthenticated != true)
                {
                    throw new UnauthorizedException();
                }
                return await service.GetMeAsync(httpContext, cancellationToken);
            })
            .WithName(nameof(GetMeEndpoint))
            .WithSummary("Get current user information based on token")
            .WithDescription("Get current user information based on token");
    }
}