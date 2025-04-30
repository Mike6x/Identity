using System.Security.Claims;
using Identity.API.EndPoints.Authorization.Handlers;
using Identity.API.Extensions;
using Identity.Core.Entities;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Identity.API.EndPoints.Authorization;

public static class AuthorizationEndpoints
{
    public static IEndpointRouteBuilder MapOpenIdConnectEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGetAuthorizeEndpoint();
        app.MapGetUserInfoEndpoint();
        app.MapGetEndSessionEndpoint();
        app.MapTokenEndpoint();
        app.MapGetTokenEndpoint();
        
        return app;
    }
}

public static class GetAuthorizeEndpoint
{
    public static RouteHandlerBuilder MapGetAuthorizeEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapGet("/authorize", AuthorizeHandler)
            .WithName(nameof(GetAuthorizeEndpoint))
            .WithSummary("Get Authorize Information")
            .WithDescription("Retrieve Authorize Information.")
            .AllowAnonymous()
            .DisableAntiforgery();
    }
    
    private static async Task<IResult> AuthorizeHandler(HttpContext httpContext,
        SignInManager<AppUser> signInManager,
        UserManager<AppUser> userManager,
        IOpenIddictScopeManager scopeManager
    )
    {
        var user = httpContext.User;
        var request = httpContext.GetOpenIddictServerRequest();

        if (user.Identity?.IsAuthenticated != true)
            return Results.Challenge();


        var claims = new List<Claim>
        {
            new Claim(Claims.Subject, user.Identity.Name)
        };

        var identity = new ClaimsIdentity(claims, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

        var appUser = await userManager.GetUserAsync(user) ??
                      throw new Exception();


        var principal = await signInManager.CreateUserPrincipalAsync(appUser);

        var emailClaim = principal.Claims.FirstOrDefault(x => x.Type.Equals(ClaimTypes.Email));
        if (emailClaim != null)
        {
            var existing = principal.Claims.FirstOrDefault(c => c.Type == Claims.Email);
            if (existing != null)
                principal.SetClaim(Claims.Email, emailClaim.Value);
            else 
                principal.AddClaim(Claims.Email, emailClaim.Value);
        }

        var scopes = request.GetScopes();
        principal.SetScopes(scopes);
        principal.SetResources(await scopeManager.ListResourcesAsync(scopes).ToListAsync());

        foreach (var claim in principal.Claims)
        {
            claim.SetDestinations(GetDestinations(claim, principal));
        }

        return Results.SignIn(principal, authenticationScheme: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    }
    
        private static IEnumerable<string> GetDestinations(Claim claim, ClaimsPrincipal principal)
    {
        // Note: by default, claims are NOT automatically included in the access and identity tokens.
        // To allow OpenIddict to serialize them, you must attach them a destination, that specifies
        // whether they should be included in access tokens, in identity tokens or in both.

        switch (claim.Type)
        {
            case OpenIddictConstants.Claims.Subject:
                yield return OpenIddictConstants.Destinations.AccessToken;
                yield return OpenIddictConstants.Destinations.IdentityToken;
                yield break;
            case OpenIddictConstants.Claims.Name:
                yield return OpenIddictConstants.Destinations.AccessToken;
                // TODO check
                //if (principal.HasScope(OpenIddictConstants.Permissions.Scopes.Profile))
                yield return OpenIddictConstants.Destinations.IdentityToken;

                yield break;
            case OpenIddictConstants.Claims.GivenName:
                yield return OpenIddictConstants.Destinations.AccessToken;
                if (principal.HasScope(OpenIddictConstants.Permissions.Scopes.Profile))
                    yield return OpenIddictConstants.Destinations.IdentityToken;
                yield break;
            case OpenIddictConstants.Claims.FamilyName:
                yield return OpenIddictConstants.Destinations.AccessToken;
                if (principal.HasScope(OpenIddictConstants.Permissions.Scopes.Profile))
                    yield return OpenIddictConstants.Destinations.IdentityToken;
                yield break;
            case OpenIddictConstants.Claims.Email:
                yield return OpenIddictConstants.Destinations.AccessToken;
                // TODO check
                //if (principal.HasScope(OpenIddictConstants.Permissions.Scopes.Email))
                yield return OpenIddictConstants.Destinations.IdentityToken;

                yield break;

            case OpenIddictConstants.Claims.Role:
                yield return OpenIddictConstants.Destinations.AccessToken;

                if (principal.HasScope(OpenIddictConstants.Permissions.Scopes.Roles))
                    yield return OpenIddictConstants.Destinations.IdentityToken;

                yield break;

            // Never include the security stamp in the access and identity tokens, as it's a secret value.
            case "AspNet.Authentication.SecurityStamp": yield break;

            default:
                yield return OpenIddictConstants.Destinations.AccessToken;
                yield break;
        }
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
        return endpoints.MapGet("/endsession", EndSessionHandler)
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
    
    private static async Task<IResult> TokenHandler(HttpContext httpContext)
    {
        var request = httpContext.GetOpenIddictServerRequest();
        var result = await httpContext.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        var principal = result.Principal;


        if (
            !(
                request.IsAuthorizationCodeGrantType() ||
                request.IsRefreshTokenGrantType()
            ))
        {
            principal = null;
        }

        if (principal != null)
            return Results.SignIn(principal, authenticationScheme: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

        return Results.Forbid(authenticationSchemes: new List<string>
        {
            OpenIddictServerAspNetCoreDefaults.AuthenticationScheme
        });

    }
}
