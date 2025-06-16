using System.Security.Claims;
using BuildingBlocks.Common.Extensions;
using Identity.Core.Entities;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using static OpenIddict.Abstractions.OpenIddictConstants;
using static Identity.Infrastructure.Services.Authorization.AuthorizationServiceHelpers;

namespace Identity.Infrastructure.Services.Authorization.Handlers;

public static class Token
{
        
    public static async Task<IResult> Handler(
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
                    Error = OpenIddictConstants.Errors.UnsupportedGrantType,
                    ErrorDescription = "The specified grant type is not supported.",
                }
            );

        var result = await httpContext.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

        var userId = result.Principal?.GetClaim(Claims.Subject);

        if (userId is null)
            return Results.UnprocessableEntity(new OpenIddictResponse
            {
                Error = OpenIddictConstants.Errors.InvalidRequest,
                ErrorDescription = "The specified user id was not found.",
            });

        var user = await userManager.FindByIdAsync(userId);

        if (user is null)
            return Results.NotFound(new OpenIddictResponse
            {
                Error = OpenIddictConstants.Errors.LoginRequired,
                ErrorDescription = "We couldn't find the requested user.",
            });

        // Ensure the user is still allowed to sign in.
        if (!await signInManager.CanSignInAsync(user))
            return Results.Forbid(
                authenticationSchemes: [OpenIddictServerAspNetCoreDefaults.AuthenticationScheme],
                properties: new(new Dictionary<string, string?>
                {
                    [OpenIddictServerAspNetCoreConstants.Properties.Error] = OpenIddictConstants.Errors.InvalidGrant,
                    [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] =
                        "The user is no longer allowed to sign in.",
                }));

        if (string.IsNullOrEmpty(userId))
            return Results.Forbid(
                authenticationSchemes: [OpenIddictServerAspNetCoreDefaults.AuthenticationScheme],
                properties: new(new Dictionary<string, string?>
                {
                    [OpenIddictServerAspNetCoreConstants.Properties.Error] = OpenIddictConstants.Errors.InvalidGrant,
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