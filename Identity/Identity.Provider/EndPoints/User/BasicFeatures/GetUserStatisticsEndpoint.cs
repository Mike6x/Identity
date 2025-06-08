using Identity.Core.Features.User;

namespace Identity.Provider.EndPoints.User.BasicFeatures;
public static class GetUserStatisticsEndpoint
{
    internal static RouteHandlerBuilder MapGetUserStatisticsEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapGet("/Statistics", (CancellationToken cancellationToken, IUserService service) 
                => service.GetUserStatisticsAsync(cancellationToken))
        .WithName(nameof(GetUserStatisticsEndpoint))
        .WithSummary("get statistics about users in the system")
        // .RequirePermission("Permissions.Users.Search")
        .WithDescription("get statistics about users in the system");
    }
}
