using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

namespace BlazorWeb.Server.Endpoints;


public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var authGroup = app.MapGroup("account").WithTags("Authentications");
        
        authGroup.MapLogInEndpoint();
        authGroup.MapLogOutEndpoint();

        return app;
    }
}

public static class LogInEndpoint
{
    public static RouteHandlerBuilder MapLogInEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapGet("/login", LoginHandler)
            .WithName(nameof(LogInEndpoint))
            .WithSummary("Login Oidc Provider")
            .WithDescription("Login User")
            .AllowAnonymous();
    }
    
    private static async Task LoginHandler(HttpContext httpContext, string returnUrl = "/")
    {
        await httpContext.ChallengeAsync(OpenIdConnectDefaults.AuthenticationScheme,
            AuthenticationPropertyHelper.GetAuthProperties(returnUrl));
    }

}

public static class LogOutEndpoint
{
    public static RouteHandlerBuilder MapLogOutEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPost("/logout", LogoutHandler)
            .WithName(nameof(LogOutEndpoint))
            .WithSummary("Log Out")
            .WithDescription("Log Out.")
            .RequireAuthorization();
    }
    
    private static async Task LogoutHandler(HttpContext httpContext, string? returnUrl= "/")
    {
        if (httpContext.Request.Cookies.Count > 0)
        {
            var siteCookies = httpContext
                .Request
                .Cookies
                .Where(c => c.Key.Contains(".AspNetCore.")
                            || c.Key.Contains("Microsoft.Authentication"));

            foreach (var cookie in siteCookies)
            {
                httpContext.Response.Cookies.Delete(cookie.Key);
            }
        }

        await httpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme,
            AuthenticationPropertyHelper.GetAuthProperties(returnUrl));

        await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    }
    
}

public static class AuthenticationPropertyHelper
{
    public static AuthenticationProperties GetAuthProperties(string? returnUrl)
    {
        const string pathBase = "/";

        if (string.IsNullOrEmpty(returnUrl))
        {
            returnUrl = pathBase;
        }
        else if (!Uri.IsWellFormedUriString(returnUrl, UriKind.Relative))
        {
            returnUrl = new Uri(returnUrl, UriKind.Absolute).PathAndQuery;
        }
        else if (returnUrl[0] != '/')
        {
            returnUrl = $"{pathBase}{returnUrl}";
        }

        return new AuthenticationProperties { RedirectUri = returnUrl };
    }
}
