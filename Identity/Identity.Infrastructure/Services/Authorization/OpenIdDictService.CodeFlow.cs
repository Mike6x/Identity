/*
 * Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
 * See https://github.com/openiddict/openiddict-core for more information concerning
 * the license and the contributors participating in this project.
 */

using System.Security.Claims;
using BuildingBlocks.Common.Extensions;
using BuildingBlocks.Exceptions;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
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
        var consentVerified = await VerifyConsentAsync(httpContext);
        if (consentVerified is not null) return consentVerified;
        
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
                authenticationSchemes: [CookieAuthenticationDefaults.AuthenticationScheme],
                properties: new AuthenticationProperties
                {
                    RedirectUri = BuildRedirectUrl(httpContext, parameters)
                });
        }
        
        var result = await httpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        
        if (!IsAuthenticated(result, request))
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

            var parameters = httpContext.Request.HasFormContentType
                                                    ? httpContext.Request.Form.ToList()
                                                    : httpContext.Request.Query.ToList();

            return Results.Challenge(
                authenticationSchemes: [CookieAuthenticationDefaults.AuthenticationScheme],
                properties: new AuthenticationProperties
                {
                    RedirectUri = BuildRedirectUrl(httpContext, parameters)
                });
        }
        
        // Retrieve the profile of the logged in user.
        
        var userEmail = result.Principal?.FindFirst(ClaimTypes.Email)?.Value ?? string.Empty;
        var user = await userManager.FindByEmailAsync(userEmail);
        if (user is null)
            return Results.NotFound(new OpenIddictResponse
            {
                Error = Errors.LoginRequired,
                ErrorDescription = "We couldn't find the requested user.",
            });

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
                
                var identity = await CreateClaimsBasedIdentity(user, application);
                
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
                identity.SetDestinations(GetDestinations);
    
                return Results.SignIn(
                    new ClaimsPrincipal(identity),
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
        var request = httpContext.GetOpenIddictServerRequest() 
                      ?? throw new InvalidOperationException("The OpenID Connect request cannot be retrieved.");
        
        var user = await userManager.GetUserAsync(httpContext.User) ??
                   throw new InvalidOperationException("The user details cannot be retrieved."); 
        
        var application 
            = await applicationManager.FindByClientIdAsync(request.ClientId ?? throw new BadRequestException("ClientId is null")) 
              ??  throw new InvalidOperationException( "Details concerning the calling client application cannot be found.");
        
        // Retrieve the permanent authorizations associated with the user and the calling client application.
        var authorizations = await authorizationManager.FindAsync(
            subject: await userManager.GetUserIdAsync(user),
            client : await applicationManager.GetIdAsync(application),
            status : Statuses.Valid,
            type   : AuthorizationTypes.Permanent,
            scopes : request.GetScopes()).ToListAsync();

        // Note: the same check is already made in the other action but is repeated
        // here to ensure a malicious user can't abuse this POST-only endpoint and
        // force it to return a valid response without the external authorization.
        if (authorizations.Count is 0 && await applicationManager.HasConsentTypeAsync(application,ConsentTypes.External))
        {
            return Results.Forbid(
                authenticationSchemes: [OpenIddictServerAspNetCoreDefaults.AuthenticationScheme],
                properties: new AuthenticationProperties( new Dictionary<string, string?>
                    {
                        [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.ConsentRequired,
                        [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] =
                            "The logged in user is not allowed to access this client application."
                    }
                ));
        }

        // Create the claims-based identity that will be used by Authorization to generate tokens.
       
        var identity = await CreateClaimsBasedIdentity(user, application);
        
        await AddUserClaimsAsync(identity, user);
        
        // Note: in this sample, the granted scopes match the requested scope
        // but you may want to allow the user to uncheck specific scopes.
        // For that, restrict the list of scopes before calling SetScopes.
        identity.SetScopes(request.GetScopes());
        identity.SetResources(await scopeManager.ListResourcesAsync(identity.GetScopes()).ToListAsync());

        // Automatically create a permanent authorization to avoid requiring explicit consent
        // for future authorization or token requests containing the same scopes.
        var authorization = authorizations.LastOrDefault();
        authorization ??= await authorizationManager.CreateAsync(
            identity: identity,
            subject: await userManager.GetUserIdAsync(user),
            client: await applicationManager.GetIdAsync(application) ?? string.Empty,
            type: AuthorizationTypes.Permanent,
            scopes: identity.GetScopes()
        );

        identity.SetAuthorizationId(await authorizationManager.GetIdAsync(authorization));
        identity.SetDestinations(GetDestinations);
        
        // Returning a SignInResult will ask Authorization to issue the appropriate access/identity tokens.
        return Results.SignIn(
                            new ClaimsPrincipal(identity), 
                            authenticationScheme: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    }

    // Notify OpenIddict that the authorization grant has been denied by the resource owner
    // to redirect the user agent to the client application using the appropriate response_mode.
    public IResult Deny()
    {
        return Results.Forbid(authenticationSchemes: [OpenIddictServerAspNetCoreDefaults.AuthenticationScheme]);
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