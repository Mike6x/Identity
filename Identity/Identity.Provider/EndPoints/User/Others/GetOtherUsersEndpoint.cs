using Identity.Core.Features.User;

namespace Identity.Provider.EndPoints.User.Others
{
    public static class  GetOtherUsersEndpoint
    {
        internal static RouteHandlerBuilder MapGetOtherUsersEndpoint(this IEndpointRouteBuilder endpoints)
        {
            return endpoints.MapGet("{userId}/otherusers", async (string userId,IUserService service, CancellationToken cancellationToken) =>
            {
                var list = await service.GetAllAsync(cancellationToken);

                return list.Where(user => user.Id.ToString() != userId).ToList();
            })
            .WithName(nameof(GetOtherUsersEndpoint))
            .WithSummary("get others")
            // .RequirePermission("Permissions.Handlers.Search")
            .WithDescription("Get list of other users");
        }
        
    }
}
