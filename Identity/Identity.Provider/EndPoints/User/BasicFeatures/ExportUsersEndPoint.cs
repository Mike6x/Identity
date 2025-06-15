using BuildingBlocks.Identity.Users.Abstractions;
using Identity.Core.Features.User;
using Identity.Core.Features.User.ExportUsers;

namespace Identity.Provider.EndPoints.User.BasicFeatures
{
    public static class ExportUsersEndpoint
    {
        internal static RouteHandlerBuilder MapExportUsersEndpoint(this IEndpointRouteBuilder endpoints)
        {
            return endpoints.MapPost("/export", (ExportUsersRequest request, IUserService service, CancellationToken cancellationToken) =>
            {
                return service.ExportAsync(request, cancellationToken);
            })
            .WithName(nameof(ExportUsersEndpoint))
            .WithSummary("Export a list of users with paging support")
            // .RequirePermission("Permissions.Handlers.Export")
            .WithDescription("Export a list of users with paging support");
        }
    }
}
