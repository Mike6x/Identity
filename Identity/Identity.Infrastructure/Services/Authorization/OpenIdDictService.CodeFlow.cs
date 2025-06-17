using System.Collections.Immutable;
using System.Security.Claims;
using BuildingBlocks.Common.Extensions;
using BuildingBlocks.Exceptions;
using Microsoft.AspNetCore;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Identity.Infrastructure.Services.Authorization;

public partial class OpenIdDictService
{
    #region Authorization code, implicit and hybrid flows
    // Note: to support interactive flows like the code flow,
    // you must provide your own authorization endpoint action:
    
    public async Task<IResult> AuthorizeAsync(HttpContext httpContext)
    {
        var request = httpContext.GetOpenIddictServerRequest() ?? 
                      throw new InvalidOperationException( "The OpenID Connect request cannot be retrieved.");
        
        // If prompt=login was specified by the client application,
        // immediately return the user agent to the login page.
        if (request.HasPromptValue(PromptValues.Login))
        {
            var prompt = string.Join(" ", request.GetPromptValues().Remove(PromptValues.Login));

            var parameters = httpContext.Request.HasFormContentType
                ? httpContext.Request.Form.Where(parameter => parameter.Key != Parameters.Prompt).ToList()
                : httpContext.Request.Query.Where(parameter => parameter.Key != Parameters.Prompt).ToList();

            parameters.Add(KeyValuePair.Create(Parameters.Prompt, new StringValues(prompt)));
            return Results.Challenge(
                authenticationSchemes: [IdentityConstants.ApplicationScheme],
                properties: new AuthenticationProperties
                {
                    RedirectUri = httpContext.Request.PathBase + httpContext.Request.Path + QueryString.Create(parameters),
                });
        }
        
        // Try to retrieve the user principal stored in the authentication cookie and redirect
        // the user agent to the login page (or to an external provider) in the following cases:
        //  - If the user principal can't be extracted or the cookie is too old.
        //  - If a max_age parameter was provided and the authentication cookie is not considered "fresh" enough.
        //
        // For scenarios where the default authentication handler configured in the ASP.NET Core
        // authentication options shouldn't be used, a specific scheme can be specified here.
        
        var result = await httpContext.AuthenticateAsync(IdentityConstants.ApplicationScheme);
        if (!result.Succeeded
                || (request.MaxAge != null
                    && result.Properties?.IssuedUtc != null
                    && DateTimeOffset.UtcNow - result.Properties.IssuedUtc > TimeSpan.FromSeconds(request.MaxAge.Value)))
        {
            // If the client application requested promptless authentication,
            // return an error indicating that the user is not logged in.
            if (request.HasPromptValue(PromptValues.None))
            {
                return Results.Forbid(
                    authenticationSchemes: [ OpenIddictServerAspNetCoreDefaults.AuthenticationScheme],
                    properties: new AuthenticationProperties(
                        new Dictionary<string, string?>
                        {
                            [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.LoginRequired,
                            [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "The user is not logged in.",
                        }));
            }

            return Results.Challenge(
                authenticationSchemes: [IdentityConstants.ApplicationScheme],
                properties: new AuthenticationProperties
                {
                   RedirectUri = httpContext.Request.PathBase 
                                 + httpContext.Request.Path 
                                 + QueryString.Create(httpContext.Request.HasFormContentType 
                                                           ? httpContext.Request.Form.ToList() 
                                                           : httpContext.Request.Query.ToList())
                });
        }
        
        // Retrieve the profile of the logged in user.
        var user = await userManager.GetUserAsync(result.Principal ?? throw new BadHttpRequestException("Principal is null"))
                            ?? throw new InvalidOperationException("The user details cannot be retrieved.");
    
        // Retrieve the application details from the database.
        var application 
            = await applicationManager.FindByClientIdAsync(request.ClientId ?? throw new BadRequestException("ClientId is null")) 
                        ??  throw new InvalidOperationException( "Details concerning the calling client application cannot be found.");
        
        var authorizations = await authorizationManager.FindAsync(
            subject: await userManager.GetUserIdAsync(user),
            client : await applicationManager.GetIdAsync(application),
            status: Statuses.Valid,
            type: AuthorizationTypes.Permanent,
            scopes : request.GetScopes()
            ).ToListAsync();
        
        switch (await applicationManager.GetConsentTypeAsync(application))
        {
            // If the consent is external (e.g when authorizations are granted by a sysadmin),
            // immediately return an error if no authorization can be found in the database.
            case ConsentTypes.External when authorizations.Count is 0:
                return Results.Forbid(
                    authenticationSchemes: [OpenIddictServerAspNetCoreDefaults.AuthenticationScheme],
                    properties: new AuthenticationProperties(
                        new Dictionary<string, string?>
                        {
                            [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.ConsentRequired,
                            [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = 
                                "The logged in user is not allowed to access this client application.",
                        }));
    
            // If the consent is implicit or if an authorization was found,
            // return an authorization response without displaying the consent form.
            case ConsentTypes.Implicit:
            case ConsentTypes.External when authorizations.Count is not 0:
            case ConsentTypes.Explicit when authorizations.Count is not 0 && !request.HasPromptValue(PromptValues.Consent):
                
                // Create the claims-based identity that will be used by Authorization to generate tokens.
                var permissions = await applicationManager.GetPermissionsAsync(application);

                var audiences = permissions.Where(x =>
                        x.StartsWith("scp") && !x.EndsWith("email") && !x.EndsWith("profile") && !x.EndsWith("roles"))
                    .Select(x => x["scp:".Length..])
                    .ToImmutableArray();
                
                var identity = new ClaimsIdentity(
                    authenticationType: TokenValidationParameters.DefaultAuthenticationType,
                    nameType: Claims.Name,
                    roleType: Claims.Role
                );
    
                // Add the claims that will be persisted in the tokens.
                // identity
                //     .SetClaim(Claims.Subject, await userManager.GetUserIdAsync(user))
                //     .SetClaim(Claims.Email, await userManager.GetEmailAsync(user))
                //     .SetClaim(Claims.Name, await userManager.GetUserNameAsync(user))
                //     .SetClaim(Claims.PreferredUsername, await userManager.GetUserNameAsync(user))
                //     .SetClaims(Claims.Role, [..(await userManager.GetRolesAsync(user))]);
                
                identity.SetClaim(Claims.Subject, user.Id.ToString())
                    .SetClaim(Claims.Email, user.Email)
                    .SetClaim(Claims.Username, user.UserName)
                    .SetClaim(Claims.Name, $"{user.FirstName} {user.LastName}")
                    .SetClaims(Claims.Audience, audiences)
                    .SetClaims(Claims.Role, [..await userManager.GetRolesAsync(user)]);
    
               await AddUserClaimsAsync(identity, user);
                
                // Note: in this sample, the granted scopes match the requested scope
                // but you may want to allow the user to uncheck specific scopes.
                // For that, simply restrict the list of scopes before calling SetScopes.
                identity.SetScopes(request.GetScopes());
                identity.SetResources(await scopeManager.ListResourcesAsync(identity.GetScopes()).ToListAsync());
    
                // Automatically create a permanent authorization to avoid requiring explicit consent
                // for future authorization or token requests containing the same scopes.
                var authorization = authorizations.LastOrDefault();
                authorization ??= await authorizationManager.CreateAsync(
                    identity: identity,
                    subject: await userManager.GetUserIdAsync(user),
                    client: await applicationManager.GetIdAsync(application) ?? throw new InvalidOperationException(),
                    type: AuthorizationTypes.Permanent,
                    scopes: identity.GetScopes()
                );
    
                identity.SetAuthorizationId(await authorizationManager.GetIdAsync(authorization));
                identity.SetDestinations(IncludeDestinationInAccessToken.Get);
    
                return Results.SignIn(
                    new ClaimsPrincipal(identity), null,
                    authenticationScheme: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme
                );
    
            // At this point, no authorization was found in the database and an error must be returned
            // if the client application specified prompt=none in the authorization request.
            case ConsentTypes.Explicit when request.HasPromptValue(PromptValues.None):
            case ConsentTypes.Systematic when request.HasPromptValue(PromptValues.None):
                return Results.Forbid(
                    authenticationSchemes: [OpenIddictServerAspNetCoreDefaults.AuthenticationScheme],
                    properties: new AuthenticationProperties(
                        new Dictionary<string, string?>
                        {
                            [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.ConsentRequired,
                            [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] 
                                = "Interactive user consent is required.",
                        }));
    
            // In every other case, render the consent form.
            default:
                var jsonData = $"{{  \"applicationName\": \"{await applicationManager.GetLocalizedDisplayNameAsync(application)}\", \"scope\": \"{request.Scope}\"  }}";
                httpContext.Session.SetString("ConsentData", jsonData);
                IEnumerable<KeyValuePair<string, StringValues>> parameters = httpContext.Request.HasFormContentType 
                                                                            ? httpContext.Request.Form 
                                                                            : httpContext.Request.Query;
                
                return Results.Redirect($"/Consent{QueryString.Create(parameters)}");
        }
    }
    
    public async Task<IResult> AcceptAsync(HttpContext httpContext)
    {
        var consentVerified = await VerifyConsentAsync(httpContext);
        if (consentVerified is not null)
            return consentVerified;
        
        var request = httpContext.GetOpenIddictServerRequest() 
                      ?? throw new InvalidOperationException("The OpenID Connect request cannot be retrieved.");
        
        // Retrieve the claims principal associated with the user code.
        var result = await httpContext.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        var principal = result.Principal;
        if (principal == null) 
            return Results.Challenge(
                authenticationSchemes: [OpenIddictServerAspNetCoreDefaults.AuthenticationScheme],
                properties: new AuthenticationProperties(new Dictionary<string, string>
                {
                    [OpenIddictServerAspNetCoreConstants.Properties.Error] = OpenIddictConstants.Errors.InvalidToken,
                    [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] =
                        "The specified access token is bound to an account that no longer exists."
                }!));

        // Retrieve the profile of the logged in user.
        var loggedInUser = await userManager.GetUserAsync(principal) 
                   ?? throw new InvalidOperationException("The user details cannot be retrieved.");

        // Retrieve the application details from the database.
        var application = await applicationManager.FindByClientIdAsync(request.ClientId ?? string.Empty) 
                          ?? throw new InvalidOperationException("Details concerning the calling client application cannot be found." );

        // Retrieve the permanent authorizations associated with the user and the calling client application.
        var authorizations = await authorizationManager.FindAsync(
            subject: await userManager.GetUserIdAsync(loggedInUser),
            client : await applicationManager.GetIdAsync(application),
            status : OpenIddictConstants.Statuses.Valid,
            type   : OpenIddictConstants.AuthorizationTypes.Permanent,
            scopes : request.GetScopes()).ToListAsync();

        // Note: the same check is already made in the other action but is repeated
        // here to ensure a malicious user can't abuse this POST-only endpoint and
        // force it to return a valid response without the external authorization.
        if (authorizations.Count is 0 
            && await applicationManager.HasConsentTypeAsync(application,OpenIddictConstants.ConsentTypes.External))
        {
            return Results.Forbid(
                properties: new AuthenticationProperties( new Dictionary<string, string>
                    {
                        [OpenIddictServerAspNetCoreConstants.Properties.Error] = OpenIddictConstants.Errors.ConsentRequired,
                        [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] =
                            "The logged in user is not allowed to access this client application.",
                    }!
                ),
                authenticationSchemes: new List<string>
                {
                    OpenIddictServerAspNetCoreDefaults.AuthenticationScheme
                });
        }

        // Create the claims-based identity that will be used by Authorization to generate tokens.
        var identity = new ClaimsIdentity(
            authenticationType: TokenValidationParameters.DefaultAuthenticationType,
            nameType: OpenIddictConstants.Claims.Name,
            roleType: OpenIddictConstants.Claims.Role
        );

        // Add the claims that will be persisted in the tokens.
        identity
            .SetClaim(OpenIddictConstants.Claims.Subject, await userManager.GetUserIdAsync(loggedInUser))
            .SetClaim(OpenIddictConstants.Claims.Email, await userManager.GetEmailAsync(loggedInUser))
            .SetClaim(OpenIddictConstants.Claims.Name, await userManager.GetUserNameAsync(loggedInUser))
            .SetClaim(OpenIddictConstants.Claims.PreferredUsername, await userManager.GetUserNameAsync(loggedInUser) )
            .SetClaims(OpenIddictConstants.Claims.Role,[.. (await userManager.GetRolesAsync(loggedInUser))]
            );

        await AddUserClaimsAsync(identity, loggedInUser);
        
        // Note: in this sample, the granted scopes match the requested scope
        // but you may want to allow the user to uncheck specific scopes.
        // For that, simply restrict the list of scopes before calling SetScopes.
        identity.SetScopes(request.GetScopes());
        identity.SetResources(await scopeManager.ListResourcesAsync(identity.GetScopes()).ToListAsync());

        // Automatically create a permanent authorization to avoid requiring explicit consent
        // for future authorization or token requests containing the same scopes.
        var authorization = authorizations.LastOrDefault();
        authorization ??= await authorizationManager.CreateAsync(
            identity: identity,
            subject: await userManager.GetUserIdAsync(loggedInUser),
            client: await applicationManager.GetIdAsync(application) ?? string.Empty,
            type: OpenIddictConstants.AuthorizationTypes.Permanent,
            scopes: identity.GetScopes()
        );

        identity.SetAuthorizationId(await authorizationManager.GetIdAsync(authorization));
        identity.SetDestinations(IncludeDestinationInAccessToken.Get);

        // Returning a SignInResult will ask Authorization to issue the appropriate access/identity tokens.
        return Results.SignIn(
            new ClaimsPrincipal(identity),
            authenticationScheme: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme
        );
    }

    public IResult Deny()
    {
        
        return Results.Forbid(authenticationSchemes: new List<string>
        {
            OpenIddictServerAspNetCoreDefaults.AuthenticationScheme
        });
    }

    private async Task<IResult?> VerifyConsentAsync(HttpContext httpContext)
    {
        if (httpContext.Request.Method != "POST")
            return null;
    
        if (httpContext.Request.Form.Any(parameter => parameter.Key == "submit.Accept"))
            return await AcceptAsync(httpContext);
    
        if (httpContext.Request.Form.Any(parameter => parameter.Key == "submit.Deny"))
            return Deny();
    
        return null;
    }
    
    #endregion

}