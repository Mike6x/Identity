using System.Security.Claims;
using Identity.API.Extensions;
using Identity.API.Models;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Identity.API.EndPoints.OpenIDConnect;

public static class OpenIdConnectEndpoints
{
    public static IEndpointRouteBuilder MapOpenIdConnectEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app
            .MapGroup("connect")
            .WithTags("OpenIdConnect")
            .WithName("OpenIdConnect");



        group.MapGet("/authorize", AuthorizeHandler)
            .WithName("Authorize")
            .AllowAnonymous()
            .DisableAntiforgery();

        group.MapPost("/token", (Delegate)TokenHandler)
            .WithName("Token")
            .AllowAnonymous()
            .DisableAntiforgery();

        group.MapGet("/userinfo", UserInfoHandler)
            .WithName("UserInfo")
            .Produces(StatusCodes.Status200OK, typeof(IEnumerable<Claim>), "application/json")
            .RequireAuthorization(policy =>
                {
                    policy.AddAuthenticationSchemes(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
                    policy.RequireAuthenticatedUser();
                }
            );

        group.MapGet("/endsession", EndSessionHandler)
            .WithName("EndSession");



        return app;
    }

    #region Handlers


    private static async Task<IResult> AuthorizeHandler(HttpContext httpContext,
           SignInManager<ApplicationUser> signInManager,
           UserManager<ApplicationUser> userManager,
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

    private static async Task<IResult> UserInfoHandler(HttpContext httpContext, UserManager<ApplicationUser> userManager)
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

    private static async Task<IResult> EndSessionHandler(HttpContext context, SignInManager<ApplicationUser> signInManager)
    {
        await signInManager.SignOutAsync();
        return Results.SignOut(authenticationSchemes: new List<string>
            {
                OpenIddictServerAspNetCoreDefaults.AuthenticationScheme
            });
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
            case "AspNet.Identity.SecurityStamp": yield break;

            default:
                yield return OpenIddictConstants.Destinations.AccessToken;
                yield break;
        }
    }



    #endregion

}
