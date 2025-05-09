using System.Security.Claims;
using Identity.Core.Entities;
using Identity.Infrastructure.Extensions;
using Identity.Provider.Extensions;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Identity.Provider.EndPoints.Authorization.Handlers;

public static class Exchange
{
    public static async Task<IResult> Handler(
        HttpContext httpContext,
        IOpenIddictApplicationManager applicationManager,
        SignInManager<AppUser> signInManager,
        UserManager<AppUser> userManager,
        IOpenIddictScopeManager scopeManager)
    {
        var request = httpContext.GetOpenIddictServerRequest() 
                      ?? throw new InvalidOperationException("The OpenID Connect request cannot be retrieved.");
                
        if (request.IsAuthorizationCodeGrantType() || request.IsDeviceCodeGrantType() || request.IsRefreshTokenGrantType()) 
            return await HandleExchangeAuthorizationAndDeviceCodeAndRefreshTokenReGrantType(httpContext, userManager, signInManager);
        
        
        if (request.IsClientCredentialsGrantType()) 
            return await HandleExchangeClientCredentialsGrantType(request, applicationManager);
        
        if (request.IsPasswordGrantType()) 
            return await HandleExchangePasswordGrantType(request, userManager, signInManager, scopeManager);


        throw new NotImplementedException("The specified grant type is not supported.");
    }

    private static async Task<IResult> HandleExchangeAuthorizationAndDeviceCodeAndRefreshTokenReGrantType(
        HttpContext httpContext,
        UserManager<AppUser> userManager,
        SignInManager<AppUser> signInManager)
    {
        var result = await httpContext.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

        // Retrieve the user profile corresponding to the refresh token.
        var user = await userManager.FindByIdAsync(result.Principal?.GetClaim(Claims.Subject) ?? string.Empty);
        if (user is null)
        {
            return Results.Forbid(
                properties: new AuthenticationProperties(new Dictionary<string, string>
                {
                    [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.InvalidGrant,
                    [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "The token is no longer valid."
                }!),
                authenticationSchemes: [OpenIddictServerAspNetCoreDefaults.AuthenticationScheme]);
        }
        
        // Ensure the user is still allowed to sign in.
        if (!await signInManager.CanSignInAsync(user))
        {
            return Results.Forbid( 
                properties: new AuthenticationProperties(new Dictionary<string, string>
                {
                    [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.InvalidGrant,
                    [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "The user is no longer allowed to sign in."
                }!),
                authenticationSchemes: [OpenIddictServerAspNetCoreDefaults.AuthenticationScheme]);
        }
        
        var identity = new ClaimsIdentity(result.Principal?.Claims,
            authenticationType: TokenValidationParameters.DefaultAuthenticationType,
            nameType: Claims.Name,
            roleType: Claims.Role);

        // Override the user claims present in the principal in case they changed since the refresh token was issued.
        identity.SetClaim(Claims.Subject, await userManager.GetUserIdAsync(user))
            .SetClaim(Claims.Email, await userManager.GetEmailAsync(user))
            .SetClaim(Claims.Name, await userManager.GetUserNameAsync(user))
            .SetClaim(Claims.PreferredUsername, await userManager.GetUserNameAsync(user))
            .SetClaims(Claims.Role, [.. await userManager.GetRolesAsync(user)]);

        identity.SetDestinations(ClaimExtensions.GetDestinations);

        // Returning a SignInResult will ask Authorization to issue the appropriate access/identity tokens.
        return Results.SignIn(
            new ClaimsPrincipal(identity), 
            authenticationScheme: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    }
    private static async Task<IResult> HandleExchangeClientCredentialsGrantType(
        OpenIddictRequest request, 
        IOpenIddictApplicationManager applicationManager
    )
    {
        {
            // Note: the client credentials are automatically validated by OpenIddict:
            // if client_id or client_secret are invalid, this action won't be invoked.
            var application =
                await applicationManager.FindByClientIdAsync(request.ClientId ?? throw new InvalidOperationException()) ??
                throw new InvalidOperationException("The application cannot be found.");
            
            // Create a new ClaimsIdentity containing the claims that will be used to create an id_token, a token or a code.
            var identity = new ClaimsIdentity(
                TokenValidationParameters.DefaultAuthenticationType, 
                Claims.Name, 
                Claims.Role);

            // Use the client_id as the subject identifier.
            identity.SetClaim(Claims.Subject, await applicationManager.GetClientIdAsync(application));
            identity.SetClaim(Claims.Name, await applicationManager.GetDisplayNameAsync(application));
            
            identity.AddClaim(new Claim(Claims.Audience, "Resourse"));
            identity.AddClaim(new Claim("some-claim", "some-value"));
            identity.SetDestinations(static claim => claim.Type switch
            {
                // Allow the "name" claim to be stored in both the access and identity tokens
                // when the "profile" scope was granted (by calling principal.SetScopes(...)).
                Claims.Name when (claim.Subject ?? throw new InvalidOperationException()).HasScope(Permissions.Scopes
                        .Profile)
                    => [Destinations.AccessToken, Destinations.IdentityToken],

                // Otherwise, only store the claim in the access tokens.
                _ => [Destinations.AccessToken]
            });
            
            return Results.SignIn(
                new ClaimsPrincipal(identity), 
                authenticationScheme: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }

    }

    private static async Task<IResult> HandleExchangePasswordGrantType(
        OpenIddictRequest request, 
        UserManager<AppUser> userManager,
        SignInManager<AppUser> signInManager,
        IOpenIddictScopeManager scopeManager)
    {
            AuthenticationProperties properties = new();

            var user = await userManager.FindByNameAsync(request.Username ?? throw new InvalidOperationException());
            if (user is null)
            { 
                properties = new AuthenticationProperties(new Dictionary<string, string>
                {
                    [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.InvalidGrant,
                    [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "User does not exist."
                }!);

                return Results.Forbid( 
                    properties: properties, 
                    authenticationSchemes: [OpenIddictServerAspNetCoreDefaults.AuthenticationScheme]);
            }

            // Check that the user can sign in and is not locked out.
            // If two-factor authentication is supported, it would also be appropriate to check that 2FA is enabled for the user
            if (!await signInManager.CanSignInAsync(user) ||
                (userManager.SupportsUserLockout && await userManager.IsLockedOutAsync(user)))
            {
                properties = new AuthenticationProperties(new Dictionary<string, string>
                {
                    [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.InvalidGrant,
                    [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "The specified user cannot sign in"
                }!);
            
                return Results.Forbid( 
                    properties: properties, 
                    authenticationSchemes:[ OpenIddictServerAspNetCoreDefaults.AuthenticationScheme]);
            }

            // Validate the username/password parameters and ensure the account is not locked out.
            var result = await signInManager.PasswordSignInAsync(user.UserName ?? string.Empty, request.Password ?? string.Empty,
                false, lockoutOnFailure: false);
            if (!result.Succeeded)
            {
                var errorString = result switch
                {
                    { IsNotAllowed: true } => "User not allowed to login. Please confirm your email",
                    { RequiresTwoFactor: true } => "User requires 2F authentication",
                    { IsLockedOut: true } => "User has been temporarily locked due to multiple unsuccessful login attempts.",
                    _ => "The username/password couple is invalid."
                };

                properties = new AuthenticationProperties(new Dictionary<string, string>
                {
                    [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.InvalidGrant,
                    [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = errorString
                }!);
            
                return Results.Forbid( 
                    properties: properties, 
                    authenticationSchemes: [OpenIddictServerAspNetCoreDefaults.AuthenticationScheme]);
            }

            // The user is now validated, so reset lockout counts, if necessary
            if (userManager.SupportsUserLockout) await userManager.ResetAccessFailedCountAsync(user);
            
            var identity = new ClaimsIdentity(
                OpenIddictServerAspNetCoreDefaults.AuthenticationScheme, 
                Claims.Name,
                Claims.Role);

            //// Getting scopes from user parameters (TokenViewModel) and adding in Authentication 
            identity.SetScopes(request.GetScopes());

            // Getting scopes from user parameters (TokenViewModel)
            // Checking in OpenIddictScopes tables for matching resources
            // Adding in Authentication
            identity.SetResources(await scopeManager.ListResourcesAsync(identity.GetScopes()).ToListAsync());

            // Add Custom claims => sub claims are mandatory
            identity.AddClaim(new Claim(Claims.Subject, user.Id.ToString()));
            identity.AddClaim(new Claim(Claims.PreferredUsername, (user.Email ?? user.UserName) ?? throw new InvalidOperationException()));
            identity.AddClaim(new Claim(Claims.Audience, "Resource"));
            identity.AddClaim(new Claim("some-claim", "some-value"));

            // Setting destinations of claims i.e., identity token or access token

            // When using this statement, custom claims aren't included in AccessToken
            // identity.SetDestinations(x => GetDestinations(x, identity));

            identity.SetDestinations(static claim => claim.Type switch
            {
                // Allow the "name" claim to be stored in both the access and identity tokens
                // when the "profile" scope was granted (by calling principal.SetScopes(...)).
                Claims.Name when (claim.Subject ?? throw new InvalidOperationException()).HasScope(
                        Permissions.Scopes.Profile)
                    => [Destinations.AccessToken, Destinations.IdentityToken],

                // Otherwise, only store the claim in the access tokens.
                _ => [Destinations.AccessToken]
            });

            // Returning a SignInResult will ask OpenIddict to issue the appropriate access/identity tokens.

            return Results.SignIn(new ClaimsPrincipal(identity), properties,
                authenticationScheme: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    }

}