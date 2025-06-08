// using Identity.Provider.EndPoints.Users.Handler;
//
// namespace Identity.Provider.EndPoints.Users;
//
// public static class UsersEndpoints
// {
//     public static IEndpointRouteBuilder MapUsersEndpoints(this IEndpointRouteBuilder app)
//     {
//         app.MapRegisterUserEndpoint();
//         // app.MapGetMeEndpoint();
//         app.MapGetUsersEndpoint();
//
//         return app;
//     }
// }
//
// public static class GetUsersEndpoint
// {
//     internal static RouteHandlerBuilder MapGetUsersEndpoint(this IEndpointRouteBuilder endpoints)
//     {
//         return endpoints.MapGet("/", GetUsers.Handler)
//             .WithName(nameof(GetUsersEndpoint))
//             .WithSummary("get all users ")
//             .RequireAuthorization()
//             // .RequirePermission("Permissions.Users.Search")
//             .WithDescription("get all users ");
//     }
// }
//
//
// public static class RegisterUserEndpoint
// {
//     internal static RouteHandlerBuilder MapRegisterUserEndpoint(this IEndpointRouteBuilder endpoints)
//     {
//         return endpoints.MapPost("/register", RegisterUser.Handler)
//             .WithName(nameof(RegisterUserEndpoint))
//             .WithSummary("register user")
//             .WithDescription("register user");
//     }
// }

// public static class GetMeEndpoint
// {
//     internal static RouteHandlerBuilder MapGetMeEndpoint(this IEndpointRouteBuilder endpoints)
//     {
//         return endpoints.MapGet("/me", LoginUser.Handler)
//             .WithName(nameof(GetMeEndpoint))
//             .WithSummary("Get current user information based on token")
//             .WithDescription("Get current user information based on token");
//     }
// }
