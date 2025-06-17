using System.Collections.Immutable;
using System.Security.Claims;
using BuildingBlocks.Common.Extensions;
using Identity.Core.Entities;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using static OpenIddict.Abstractions.OpenIddictConstants;
using static Identity.Infrastructure.Services.Authorization.AuthorizationServiceHelpers;

namespace Identity.Infrastructure.Services.Authorization.Handlers;

public static class Authorize
{
    public static async Task<IResult> Handler(
        HttpContext context,
        UserManager<AppUser> userManager,
        IOpenIddictApplicationManager applicationManager,
        IOpenIddictScopeManager scopeManager,
        IOpenIddictAuthorizationManager authorizationManager
    )
    {
        var request = context.GetOpenIddictServerRequest();

        if (request is null)
            return Results.BadRequest(new OpenIddictResponse
            {
                Error = Errors.InvalidRequest,
                ErrorDescription = "The OpenID Connect request cannot be retrieved.",
            });

        var parameters = ParseOAuthParameters(context, [Parameters.Prompt]);

        var result = await context.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        if (!IsAuthenticated(result, request))
            return Results.Challenge(
                new AuthenticationProperties
                {
                    RedirectUri = BuildRedirectUrl(context.Request, parameters),
                },
                [CookieAuthenticationDefaults.AuthenticationScheme]
            );

        var app = await applicationManager.FindByClientIdAsync(request.ClientId ?? "");
        if (app is null)
            return Results.NotFound(new OpenIddictResponse
            {
                Error = Errors.InvalidClient,
                ErrorDescription = "The specified client was not found.",
            });

        var permissions = await applicationManager.GetPermissionsAsync(app);

        var audiences = permissions.Where(x =>
                x.StartsWith("scp") && !x.EndsWith("email") && !x.EndsWith("profile") && !x.EndsWith("roles"))
            .Select(x => x["scp:".Length..])
            .ToImmutableArray();

        var userEmail = result.Principal?.FindFirst(ClaimTypes.Email)!.Value ?? string.Empty;

        var user = await userManager.FindByEmailAsync(userEmail);

        if (user is null)
            return Results.NotFound(new OpenIddictResponse
            {
                Error = Errors.LoginRequired,
                ErrorDescription = "We couldn't find the requested user.",
            });

        //  Retrieve the permanent authorizations associated with the user and the calling client application.
        var authorizationsList = await authorizationManager.FindAsync(
            await userManager.GetUserIdAsync(user),
            await applicationManager.GetIdAsync(app),
            Statuses.Valid,
            AuthorizationTypes.Permanent,
            request.GetScopes()).ToListAsync();
        
        var authorizations = await authorizationManager.FindAsync(
            subject: await userManager.GetUserIdAsync(user),
            client : await applicationManager.GetIdAsync(app),
            status: Statuses.Valid,
            type: AuthorizationTypes.Permanent,
            scopes : request.GetScopes()
        ).ToListAsync();

        // Note: the same check is already made in the other action but is repeated
        // here to ensure a malicious user can't abuse this POST-only endpoint and
        // force it to return a valid response without the external authorization.
        if (authorizations.Count is 0 && await applicationManager.HasConsentTypeAsync(app, ConsentTypes.External))
            return Results.Forbid(
                authenticationSchemes: [OpenIddictServerAspNetCoreDefaults.AuthenticationScheme],
                properties: new AuthenticationProperties(new Dictionary<string, string?>
                {
                    [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.ConsentRequired,
                    [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] =
                        "The logged in user is not allowed to access this client application.",
                }));

        var identity = new ClaimsIdentity(
            TokenValidationParameters.DefaultAuthenticationType,
            ClaimTypes.Name,
            ClaimTypes.Role
        );

        identity
            .SetClaim(Claims.Subject, user.Id.ToString())
            .SetClaim(Claims.Email, user.Email)
            .SetClaim(Claims.Username, user.UserName)
            .SetClaim(Claims.Name, $"{user.FirstName} {user.LastName}")
            .SetClaims(Claims.Audience, audiences)
            .SetClaims(Claims.Role, [..await userManager.GetRolesAsync(user)]);

        identity.SetScopes(request.GetScopes());
        identity.SetResources(await scopeManager.ListResourcesAsync(identity.GetScopes()).ToListAsync());

        // Automatically create a permanent authorization to avoid requiring explicit consent
        // for future authorization or token requests containing the same scopes.
        var authorization = authorizations.LastOrDefault();
        authorization ??= await authorizationManager.CreateAsync(
            identity,
            user.Id.ToString(),
            (await applicationManager.GetIdAsync(app))!,
            AuthorizationTypes.Permanent,
            identity.GetScopes());

        identity.SetAuthorizationId(await authorizationManager.GetIdAsync(authorization));
        identity.SetDestinations(c => GetDestinations(identity, c)); //?

        return Results.SignIn(
            new(identity),
            null,
            OpenIddictServerAspNetCoreDefaults.AuthenticationScheme
        );
    }
}