using Identity.Core.Features.Role;
using Identity.Core.Features.Role.SearchRoles;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Identity.Infrastructure.Endpoints.Role;

public static class SearchRolesEndpoint
{
    internal static RouteHandlerBuilder MapSearchRolesEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPost("/search", (SearchRolesRequest request, IRoleService service, CancellationToken cancellationToken) =>
            {
                return service.SearchAsync(request, cancellationToken);
            })
            .WithName(nameof(SearchRolesEndpoint))
            .WithSummary("get a list of roles with paging support")
            // .RequirePermission("Permissions.Roles.Search")
            .WithDescription("get a list of roles with paging support");
    }
}