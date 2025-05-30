// using Microsoft.AspNetCore.Authentication;
// using Microsoft.AspNetCore.Authentication.Cookies;
// using Microsoft.AspNetCore.Authentication.OpenIdConnect;
// using Microsoft.Extensions.Primitives;
//
// namespace BlazorWeb.Server.Endpoints;
//
// public static class LoginLogoutEndpoints
// {
//     public static WebApplication MapLoginLogoutEndpoints(this WebApplication app)
//     {
//         app.MapGet("/login", async context =>
//         {
//             var returnUrl = context.Request.Query["returnUrl"];
//
//             await context.ChallengeAsync(OpenIdConnectDefaults.AuthenticationScheme, new AuthenticationProperties
//             {
//                 RedirectUri = returnUrl == StringValues.Empty ? "/" : returnUrl.ToString()
//             });
//         }).AllowAnonymous();
//
//         app.MapPost("/logout", async context =>
//         {
//             if (context.Request.Cookies.Count > 0)
//             {
//                 var siteCookies = context
//                     .Request
//                     .Cookies
//                     .Where(c => c.Key.Contains(".AspNetCore.")
//                                 || c.Key.Contains("Microsoft.Authentication"));
//
//                 foreach (var cookie in siteCookies)
//                 {
//                     context.Response.Cookies.Delete(cookie.Key);
//                 }
//             }
//             if (context.User.Identity?.IsAuthenticated ?? false)
//             {
//                 await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
//                 await context.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);
//             }
//             else
//             {
//                 context.Response.Redirect("/");
//             }
//         });
//
//         return app;
//     }
//
// }