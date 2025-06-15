using BuildingBlocks.Identity.Users.Abstractions;
using Identity.Core.Features.User;
using Identity.Core.Features.User.SearchUsers;

namespace Identity.Provider.EndPoints.User.BasicFeatures
{

    public static class SearchUsersEndpoint
    {
        internal static RouteHandlerBuilder MapSearchUsersEndpoint(this IEndpointRouteBuilder endpoints)
        {
            return endpoints.MapPost("/search", (SearchUsersRequest request, IUserService service, CancellationToken cancellationToken) =>
            {
                return service.SearchAsync(request, cancellationToken);
            })
            .WithName(nameof(SearchUsersEndpoint))
            .WithSummary("get a list of users with paging support")
            // .RequirePermission("Permissions.Users.Search")
            .WithDescription("get a list of users with paging support");
        }
    }

}