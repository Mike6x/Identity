using Identity.Core.Entities;
using Identity.Core.Features.Authorization;
using Identity.Infrastructure.Services.Authorization.Handlers;
using Microsoft.AspNetCore.Identity;
using OpenIddict.Server.AspNetCore;

namespace Identity.Provider.EndPoints.Authorization;

public static class AuthorizationEndpoints
{
    public static IEndpointRouteBuilder MapOpenIdConnectEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGetAuthorizeEndpoint();
        app.MapAuthorizeEndpoint();
        
        app.MapAcceptAuthorizationGrantEndpoint();
        app.MapDenyAuthorizationGrantEndpoint();
        
        app.MapGetTokenEndpoint();
        app.MapTokenEndpoint();
        
        app.MapGetVerifyConnectionEndpoint();
        app.MapVerifyConnectionEndpoint();
        app.MapDenyVerifyConnectionEndpoint();

        app.MapGetEndSessionEndpoint();
        app.MapEndSessionEndpoint();
        app.MapGetSignOutEndpoint();
        app.MapSignOutEndpoint();
        
        app.MapGetUserInfoEndpoint();
        app.MapUserInfoEndpoint();
        
        return app;
    }
}

#region Authorization code, implicit and hybrid flows
// Note: to support interactive flows like the code flow,
// you must provide your own authorization endpoint action:

public static class GetAuthorizeEndpoint
{
    public static RouteHandlerBuilder MapGetAuthorizeEndpoint(this IEndpointRouteBuilder endpoints)
    {
        // return endpoints.MapGet("/authorize", 
        //         (HttpContext httpContext, 
        //         IAuthorizationService service, 
        //         CancellationToken cancellationToken) => service.AuthorizeAsync(httpContext))
        return endpoints.MapGet("/authorize", Authorize.Handler)
            .WithName(nameof(GetAuthorizeEndpoint))
            .WithSummary("Get Authorize Information")
            .WithDescription("Retrieve Authorize Information.")
            .AllowAnonymous()
            .DisableAntiforgery();
    }
}

public static class AuthorizeEndpoint
{
    public static RouteHandlerBuilder MapAuthorizeEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPost("/authorize", 
                (HttpContext httpContext, 
                IAuthorizationService service, 
                CancellationToken cancellationToken) => service.AuthorizeAsync(httpContext))
            .WithName(nameof(AuthorizeEndpoint))
            .WithSummary("Get Authorize Information")
            .WithDescription("Retrieve Authorize Information.")
            .AllowAnonymous()
            .DisableAntiforgery();
    }
}

public static class AcceptAuthorizationGrantEndpoint
{
    public static RouteHandlerBuilder MapAcceptAuthorizationGrantEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPost("/authorize/accept", (HttpContext httpContext, IAuthorizationService service, CancellationToken cancellationToken) 
                => service.AcceptAsync(httpContext))
            .WithName(nameof(AcceptAuthorizationGrantEndpoint))
            .WithSummary("Deny authorization grant")
            .WithDescription("Deny authorization grant")
            .RequireAuthorization();
    }
}

// Notify Authorization that the authorization grant has been denied by the resource owner
// to redirect the user agent to the client application using the appropriate response_mode.
public static class DenyAuthorizationGrantEndpoint
{
    public static RouteHandlerBuilder MapDenyAuthorizationGrantEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPost("/authorize/deny", (IAuthorizationService service, CancellationToken cancellationToken) 
                => service.Deny())
            .WithName(nameof(DenyAuthorizationGrantEndpoint))
            .WithSummary("Deny authorization grant")
            .WithDescription("Deny authorization grant")
            .RequireAuthorization();
    }
}

#endregion

#region Password, authorization code, device and refresh token flows.
// Note: to support non-interactive flows like password,
// you must provide your own token endpoint action:

public static class GetTokenEndpoint
{
    public static RouteHandlerBuilder MapGetTokenEndpoint(this IEndpointRouteBuilder endpoints)
    {
        // return endpoints.MapGet("/token", Exchange.Handler)
        return endpoints.MapGet("/token", ( HttpContext httpContext, 
                IAuthorizationService service, 
                CancellationToken cancellationToken) => service.ExchangeAsync(httpContext))
            .WithName(nameof(GetTokenEndpoint))
            .WithSummary("Retrieve Access Token.")
            .WithDescription("Retrieve Access Token.")
            .AllowAnonymous()
            .DisableAntiforgery<RouteHandlerBuilder>();
    }
}

public static class TokenEndpoint
{
    public static RouteHandlerBuilder MapTokenEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPost("/token", ( HttpContext httpContext, 
                IAuthorizationService service, 
                CancellationToken cancellationToken) => service.ExchangeAsync(httpContext))
            .WithName(nameof(TokenEndpoint))
            .WithSummary("Retrieve Access Token.")
            .WithDescription("Retrieve Access Token.")
            .AllowAnonymous()
            .DisableAntiforgery<RouteHandlerBuilder>();
    }
}

#endregion

#region Device flow
// Note: to support the device flow, you must provide your own verification endpoint action:

public static class GetVerifyConnectionEndpoint
{
    public static RouteHandlerBuilder MapGetVerifyConnectionEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapGet("/verify",(HttpContext httpContext, IAuthorizationService service, CancellationToken cancellationToken) 
                => service.VerifyAsync(httpContext))
            .WithName(nameof(GetVerifyConnectionEndpoint))
            .WithSummary("Verify connection")
            .WithDescription("Verify connection")
            .RequireAuthorization();
    }
}
public static class VerifyConnectionEndpoint
{
    public static RouteHandlerBuilder MapVerifyConnectionEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPost("/verify",(HttpContext httpContext, IAuthorizationService service, CancellationToken cancellationToken) 
                => service.VerifyAcceptAsync(httpContext))
            .WithName(nameof(VerifyConnectionEndpoint))
            .WithSummary("Accept verify connection ")
            .WithDescription("Accept verify connection ")
            .RequireAuthorization();
    }
}

public static class DenyVerifyConnectionEndpoint
{
    public static RouteHandlerBuilder MapDenyVerifyConnectionEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPost("/verify/deny",(IAuthorizationService service, CancellationToken cancellationToken) 
                => service.VerifyDeny())
            .WithName(nameof(DenyVerifyConnectionEndpoint))
            .WithSummary("Deny verify connection ")
            .WithDescription("Deny verify connection ")
            .RequireAuthorization();
    }
}

#endregion

#region  Logout support for interactive flows like code and implicit
// Note: the logout action is only useful when implementing interactive
// flows like the authorization code flow or the implicit flow.


public static class GetEndSessionEndpoint
{
    public static RouteHandlerBuilder MapGetEndSessionEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapGet("/logout", (IAuthorizationService service, CancellationToken cancellationToken) 
                => service.EndSessionAsync())
            .WithName(nameof(GetEndSessionEndpoint))
            .WithSummary("Terminate Session")
            .WithDescription("Terminate the session.");
    }
}

public static class EndSessionEndpoint
{
    public static RouteHandlerBuilder MapEndSessionEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPost("/logout", (IAuthorizationService service, CancellationToken cancellationToken) 
                => service.EndSessionAsync())
            .WithName(nameof(EndSessionEndpoint))
            .WithSummary("Terminate Session")
            .WithDescription("Terminate the session.");
    }
}

public static class GetSignOutEndpoint
{
    public static RouteHandlerBuilder MapGetSignOutEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapGet("/signout", SignOut.Handler)
            .WithName(nameof(GetSignOutEndpoint))
            .WithSummary("Terminate Session")
            .WithDescription("Terminate the session.");
    }
}
public static class SignOutEndpoint
{
    public static RouteHandlerBuilder MapSignOutEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPost("/signout", SignOut.Handler)
            .WithName(nameof(SignOutEndpoint))
            .WithSummary("Terminate Session")
            .WithDescription("Terminate the session.");
    }
}
public static class SignOut
{
    public static async Task<IResult> Handler(HttpContext context, SignInManager<AppUser> signInManager)
    {
        await signInManager.SignOutAsync();
        return Results.SignOut(authenticationSchemes: new List<string>
        {
            OpenIddictServerAspNetCoreDefaults.AuthenticationScheme
        });
    }
}
#endregion

#region User Infomation

public static class GetUserInfoEndpoint
{
    public static RouteHandlerBuilder MapGetUserInfoEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapGet("/userinfo", UserInfo.Handler)
            .WithName(nameof(GetUserInfoEndpoint))
            .WithSummary("Retrieve User Info.")
            .WithDescription("Retrieve User Info.")
            //.Produces("application/json")
            .RequireAuthorization(policy =>
                {
                    policy.AddAuthenticationSchemes(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
                    policy.RequireAuthenticatedUser();
                }
            );
    }
}

public static class UserInfoEndpoint
{
    public static RouteHandlerBuilder MapUserInfoEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPost("/userinfo", UserInfo.Handler)
            .WithName(nameof(UserInfoEndpoint))
            .WithSummary("Retrieve User Info.")
            .WithDescription("Retrieve User Info.")
            //.Produces("application/json")
            .RequireAuthorization(policy =>
                {
                    policy.AddAuthenticationSchemes(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
                    policy.RequireAuthenticatedUser();
                }
            );
    }
}

#endregion