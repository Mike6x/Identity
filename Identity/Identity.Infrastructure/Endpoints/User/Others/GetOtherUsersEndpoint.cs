using BuildingBlocks.Identity.Users.Abstractions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Identity.Infrastructure.Endpoints.User.Others
{
    public static class  GetOtherUsersEndpoint
    {
        internal static RouteHandlerBuilder MapGetOtherUsersEndpoint(this IEndpointRouteBuilder endpoints)
        {
            return endpoints.MapGet("{userId}/others", async (string userId,IUserService service, CancellationToken cancellationToken) =>
            {
                var list = await service.GetAllAsync(cancellationToken);

                return list.Where(user => user.Id.ToString() != userId).ToList();
            })
            .WithName(nameof(GetOtherUsersEndpoint))
            .WithSummary("get others")
            .WithDescription("Get list of other users"
            // .RequirePermission("Permissions.Users.Search")
            );
        }
        
    }
}
