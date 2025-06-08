using System.Security.Claims;
using BuildingBlocks.Common.Extensions;
using Identity.Core.Entities;
using Identity.Provider.Endpoints.Authorization.Handlers;
using Identity.Provider.EndPoints.Authorization.Handlers;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using static OpenIddict.Abstractions.OpenIddictConstants;
using static Identity.Infrastructure.Services.Authorization.AuthorizationService;

namespace Identity.Provider.EndPoints.Authorization;

public static class AuthorizationEndpoints
{
    public static IEndpointRouteBuilder MapOpenIdConnectEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGetAuthorizeEndpoint();
        app.MapAuthorizeEndpoint();
        
        app.MapGetUserInfoEndpoint();
        
        app.MapGetEndSessionEndpoint();
        app.MapEndSessionEndpoint();
        
        app.MapTokenEndpoint();
        app.MapGetTokenEndpoint();
        
        return app;
    }
}

public static class GetAuthorizeEndpoint
{
    public static RouteHandlerBuilder MapGetAuthorizeEndpoint(this IEndpointRouteBuilder endpoints)
    {
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
        return endpoints.MapPost("/authorize", Authorize.Handler)
            .WithName(nameof(AuthorizeEndpoint))
            .WithSummary("Get Authorize Information")
            .WithDescription("Retrieve Authorize Information.")
            .AllowAnonymous()
            .DisableAntiforgery();
    }
}

public static class GetUserInfoEndpoint
{
    public static RouteHandlerBuilder MapGetUserInfoEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapGet("/userinfo", UserInfoHandler)
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
    
    private static async Task<IResult> UserInfoHandler(HttpContext httpContext, UserManager<AppUser> userManager)
    {
        //https://github.com/openiddict/openiddict-samples/blob/dev/samples/Dantooine/Dantooine.Server/Controllers/UserinfoController.cs
        var user = httpContext.User;

        var request = httpContext.GetOpenIddictServerRequest();

        var result = await httpContext.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        var principal = result.Principal;

        if (principal == null)
            return Results.Unauthorized();

        var applicationUser = await userManager.GetUserAsync(principal);
        var claims = new Dictionary<string, object>(StringComparer.Ordinal)
        {
            // Note: the "sub" claim is a mandatory claim and must be included in the JSON response.
            [Claims.Subject] = await userManager.GetUserIdAsync(applicationUser)
        };

        if (user.HasScope(Scopes.Email))
        {
            claims[Claims.Email] = await userManager.GetEmailAsync(applicationUser);
            claims[Claims.EmailVerified] = await userManager.IsEmailConfirmedAsync(applicationUser);
        }

        if (user.HasScope(Scopes.Phone))
        {
            claims[Claims.PhoneNumber] = await userManager.GetPhoneNumberAsync(applicationUser);
            claims[Claims.PhoneNumberVerified] = await userManager.IsPhoneNumberConfirmedAsync(applicationUser);
        }

        if (user.HasScope(Scopes.Roles))
        {
            claims[Claims.Role] = await userManager.GetRolesAsync(applicationUser);
        }

        return Results.Ok(claims);
    }
}

public static class GetEndSessionEndpoint
{
    public static RouteHandlerBuilder MapGetEndSessionEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapGet("/logout", EndSessionHandler)
            .WithName(nameof(GetEndSessionEndpoint))
            .WithSummary("Terminate Session")
            .WithDescription("Terminate the session.");
    }
    
    private static async Task<IResult> EndSessionHandler(HttpContext context, SignInManager<AppUser> signInManager)
    {
        await signInManager.SignOutAsync();
        return Results.SignOut(authenticationSchemes: new List<string>
        {
            OpenIddictServerAspNetCoreDefaults.AuthenticationScheme
        });
    }
}

public static class EndSessionEndpoint
{
    public static RouteHandlerBuilder MapEndSessionEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPost("/logout", EndSessionHandler)
            .WithName(nameof(EndSessionEndpoint))
            .WithSummary("Terminate Session")
            .WithDescription("Terminate the session.");
    }
    
    private static async Task<IResult> EndSessionHandler(HttpContext context, SignInManager<AppUser> signInManager)
    {
        await signInManager.SignOutAsync();
        return Results.SignOut(authenticationSchemes: new List<string>
        {
            OpenIddictServerAspNetCoreDefaults.AuthenticationScheme
        });
    }
}

public static class TokenEndpoint
{
    public static RouteHandlerBuilder MapTokenEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPost("/token", Exchange.Handler)
            .WithName(nameof(TokenEndpoint))
            .WithSummary("Retrieve Access Token.")
            .WithDescription("Retrieve Access Token.")
            .AllowAnonymous()
            .DisableAntiforgery<RouteHandlerBuilder>();
    }
}

public static class GetTokenEndpoint
{
    public static RouteHandlerBuilder MapGetTokenEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapGet("/token", (Delegate)TokenHandler)
            .WithName(nameof(GetTokenEndpoint))
            .WithSummary("Retrieve Access Token.")
            .WithDescription("Retrieve Access Token.")
            .AllowAnonymous()
            .DisableAntiforgery<RouteHandlerBuilder>();
    }
    
    private static async Task<IResult> TokenHandler(
        HttpContext httpContext,
        IOpenIddictApplicationManager applicationManager,
        SignInManager<AppUser> signInManager,
        UserManager<AppUser> userManager,
        IOpenIddictScopeManager scopeManager)
    {
        var request = httpContext.GetOpenIddictServerRequest() ??
                      throw new InvalidOperationException("The OpenID Connect request cannot be retrieved.");

        if (!request.IsAuthorizationCodeGrantType() && !request.IsRefreshTokenGrantType())
            return Results.BadRequest(
                new OpenIddictResponse
                {
                    Error = Errors.UnsupportedGrantType,
                    ErrorDescription = "The specified grant type is not supported.",
                }
            );

        var result = await httpContext.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

        var userId = result.Principal?.GetClaim(Claims.Subject);

        if (userId is null)
            return Results.UnprocessableEntity(new OpenIddictResponse
            {
                Error = Errors.InvalidRequest,
                ErrorDescription = "The specified user id was not found.",
            });

        var user = await userManager.FindByIdAsync(userId);

        if (user is null)
            return Results.NotFound(new OpenIddictResponse
            {
                Error = Errors.LoginRequired,
                ErrorDescription = "We couldn't find the requested user.",
            });

        // Ensure the user is still allowed to sign in.
        if (!await signInManager.CanSignInAsync(user))
            return Results.Forbid(
                authenticationSchemes: [OpenIddictServerAspNetCoreDefaults.AuthenticationScheme],
                properties: new(new Dictionary<string, string?>
                {
                    [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.InvalidGrant,
                    [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] =
                        "The user is no longer allowed to sign in.",
                }));

        if (string.IsNullOrEmpty(userId))
            return Results.Forbid(
                authenticationSchemes: [OpenIddictServerAspNetCoreDefaults.AuthenticationScheme],
                properties: new(new Dictionary<string, string?>
                {
                    [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.InvalidGrant,
                    [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "Cannot find user from token",
                })
            );

        var identity = new ClaimsIdentity(
            TokenValidationParameters.DefaultAuthenticationType,
            ClaimTypes.Name,
            ClaimTypes.Role
        );

        // Override the user claims present in the principal in case they
        // changed since the authorization code/refresh token was issued.
        identity.SetClaim(Claims.Subject, userId)
            .SetClaim(Claims.Email, user.Email)
            .SetClaim(Claims.Username, user.UserName)
            .SetClaim(Claims.Name, $"{user.FirstName} {user.LastName}")
            .SetClaims(Claims.Role, [..await userManager.GetRolesAsync(user)]);

        identity.SetDestinations(c => GetDestinations(identity, c));

        identity.SetScopes(request.GetScopes());

        identity.SetResources(await scopeManager.ListResourcesAsync(identity.GetScopes()).ToListAsync());

        // Returning a SignInResult will ask OpenIddict to issue the appropriate access/identity tokens
        return Results.SignIn(new(identity), null, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    }
}

