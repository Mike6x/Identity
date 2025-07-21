/*
// * Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// * See https://github.com/openiddict/openiddict-core for more information concerning
// * the license and the contributors participating to this project.
// */

using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Identity.Infrastructure.Services.Authorization;

public partial class OpenIdDictService
{
    public async Task<IResult> UserInfoAsync(HttpContext httpContext)
    {

        _ = httpContext.GetOpenIddictServerRequest() 
            ?? throw new InvalidOperationException("The OpenID Connect request cannot be retrieved.");

        var result = await httpContext.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        var user = result.Principal;
        if (user == null) return Results.Unauthorized();

        var loggedInUser = await userManager.GetUserAsync(user);
        if (loggedInUser is null)
        {
            var properties= new AuthenticationProperties(new Dictionary<string, string?>
            {
                [OpenIddictServerAspNetCoreConstants.Properties.Error] = OpenIddictConstants.Errors.InvalidToken,
                [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] =
                    "The specified access token is bound to an account that no longer exists."
            });

            return Results.Challenge(properties, [OpenIddictServerAspNetCoreDefaults.AuthenticationScheme]);
        }

        var claims = new Dictionary<string, object>(StringComparer.Ordinal)
        {
            // Note: the "sub" claim is a mandatory claim and must be included in the JSON response.
            [Claims.Subject] = await userManager.GetUserIdAsync(loggedInUser)
        };

        if (user.HasScope(Scopes.Email))
        {
            claims[Claims.Email] = await userManager.GetEmailAsync(loggedInUser) ?? string.Empty;
            claims[Claims.EmailVerified] = await userManager.IsEmailConfirmedAsync(loggedInUser);
        }

        if (user.HasScope(Scopes.Phone))
        {
            claims[Claims.PhoneNumber] = await userManager.GetPhoneNumberAsync(loggedInUser) ?? string.Empty;
            claims[Claims.PhoneNumberVerified] = await userManager.IsPhoneNumberConfirmedAsync(loggedInUser);
        }

        if (user.HasScope(Scopes.Roles))
        {
            claims[Claims.Role] = await userManager.GetRolesAsync(loggedInUser);
        }
        // Note: the complete list of standard claims supported by the OpenID Connect specification
        // can be found here: http://openid.net/specs/openid-connect-core-1_0.html#StandardClaims
        
        return Results.Ok(claims);
    }
    
}