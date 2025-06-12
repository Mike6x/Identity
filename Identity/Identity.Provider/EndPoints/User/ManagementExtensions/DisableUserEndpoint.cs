// using Identity.Core.Features.User;
//
// namespace Identity.Provider.EndPoints.User.ManagementExtensions;
// public static class DisableUserEndpoint
// {
//     internal static RouteHandlerBuilder MapDisableUserEndpoint(this IEndpointRouteBuilder endpoints)
//     {
//         return endpoints.MapDelete("/disable/{id:guid}", (string id, IUserService service) =>
//         {
//             return service.DisableAsync(id);
//         })
//         .WithName(nameof(DisableUserEndpoint))
//         .WithSummary("Disable user profile")
//         // .RequirePermission("Permissions.Users.Delete")
//         .WithDescription("disable user profile");
//     }
// }
