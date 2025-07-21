/*
 * Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
 * See https://github.com/openiddict/openiddict-core for more information concerning
 * the license and the contributors participating in this project.
 */

using System.Collections.Immutable;
using System.Security.Claims;
using Identity.Core.Entities;
using Identity.Core.Features.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Identity.Infrastructure.Services.Authorization;

public sealed partial class OpenIdDictService(
    IOpenIddictApplicationManager applicationManager,
    IOpenIddictAuthorizationManager authorizationManager,
    IOpenIddictScopeManager scopeManager,
    SignInManager<AppUser> signInManager,
    UserManager<AppUser> userManager,
    RoleManager<AppRole> roleManager) : IAuthorizationService
{
    private static IEnumerable<string> GetDestinations(Claim claim)
    {
        // Note: by default, claims are NOT automatically included in the access and identity tokens.
        // To allow Authorization to serialize them, you must attach them to a destination that specifies
        // whether they should be included in access tokens, in identity tokens or in both.
       
        switch (claim.Type)
        {
            case Claims.Name or Claims.FamilyName or Claims.Username:
                yield return Destinations.AccessToken;

                if (claim.Subject != null && claim.Subject.HasScope(Scopes.Profile))
                    yield return Destinations.IdentityToken;

                yield break;

            case Claims.Email:
                yield return Destinations.AccessToken;

                if (claim.Subject != null && claim.Subject.HasScope(Scopes.Email))
                    yield return Destinations.IdentityToken;

                yield break;

            case Claims.Role:
                yield return Destinations.AccessToken;

                if (claim.Subject != null && claim.Subject.HasScope(Scopes.Roles))
                    yield return Destinations.IdentityToken;

                yield break;

            // Never include the security stamp in the access and identity tokens, as it's a secret value.
            case "AspNet.Identity.SecurityStamp":
                yield break;

            default:
              
                if (claim.Properties.TryGetValue("IncludeInAccessToken", out var property))
                {
                    if (bool.TryParse(property, out var includeInAccessToken) && includeInAccessToken)
                    {
                        yield return Destinations.AccessToken;
                    }                        
                }
                
                if (claim.Properties.TryGetValue("IncludeInIdentityToken", out var claimProperty))
                {                       
                    if (bool.TryParse(claimProperty, out var includeInIdentityToken)
                        && includeInIdentityToken)
                    {
                        yield return Destinations.IdentityToken;
                    }
                }                  
                yield break;
        }
    }
    
    // private static IEnumerable<string> GetDestinations(Claim claim, ClaimsPrincipal principal)
    // {
    //     // Note: by default, claims are NOT automatically included in the access and identity tokens.
    //     // To allow OpenIddict to serialize them, you must attach them to a destination that specifies
    //     // whether they should be included in access tokens, in identity tokens or in both.
    //
    //     switch (claim.Type)
    //     {
    //         case Claims.Name:
    //             yield return Destinations.AccessToken;
    //
    //             if (principal.HasScope(Scopes.Profile))
    //                 yield return Destinations.IdentityToken;
    //
    //             yield break;
    //
    //         case Claims.Email:
    //             yield return Destinations.AccessToken;
    //
    //             if (principal.HasScope(Scopes.Email))
    //                 yield return Destinations.IdentityToken;
    //
    //             yield break;
    //
    //         case Claims.Role:
    //             yield return Destinations.AccessToken;
    //
    //             if (principal.HasScope(Scopes.Roles))
    //                 yield return Destinations.IdentityToken;
    //
    //             yield break;
    //
    //         // Never include the security stamp in the access and identity tokens, as it's a secret value.
    //         case "AspNet.Identity.SecurityStamp": yield break;
    //
    //         default:
    //             yield return Destinations.AccessToken;
    //             yield break;
    //     }
    // }
    
    private async Task AddUserClaimsAsync(ClaimsIdentity claimsIdentity, AppUser user)
    {
        foreach(var claim in await userManager.GetClaimsAsync(user))
        {
            claimsIdentity.AddClaim(claim);
        }
        foreach(var assignedRole in await userManager.GetRolesAsync(user))
        {
            var role = await roleManager.FindByNameAsync(assignedRole);
            if(role != null) claimsIdentity.AddClaims(await roleManager.GetClaimsAsync(role));
        }
    }
    
    private async Task<ClaimsIdentity> CreateClaimsBasedIdentity(AppUser user, IEnumerable<Claim>? claims, object? application)
    {
        var identity = claims == null
            ? new ClaimsIdentity(
                authenticationType: TokenValidationParameters.DefaultAuthenticationType,
                nameType: Claims.Name,
                roleType: Claims.Role)
            : new ClaimsIdentity( 
                claims: claims, 
                authenticationType: TokenValidationParameters.DefaultAuthenticationType,
                nameType: Claims.Name,
                roleType: Claims.Role);
        
        // Override the user claims present in the principal in case they
        // changed since the authorization code/refresh token was issued.
        
        identity.SetClaim(Claims.Subject, user.Id.ToString())
            .SetClaim(Claims.Email, user.Email)
            .SetClaim(Claims.Username, user.UserName)
            
            .SetClaim(Claims.FamilyName, user.LastName)
            .SetClaim(Claims.Name, user.FirstName)
            
            .SetClaims(Claims.Role, [..await userManager.GetRolesAsync(user)]);
        
        // Create the claims-based identity that will be used by Authorization to generate tokens.
        if (application is not null)
        {
            var permissions = await applicationManager.GetPermissionsAsync(application);

            var audiences = permissions.Where(x =>
                    x.StartsWith("scp") && !x.EndsWith("email") && !x.EndsWith("profile") && !x.EndsWith("roles"))
                .Select(x => x["scp:".Length..])
                .ToImmutableArray();
            
            identity
                .SetClaims(Claims.Audience, audiences);
        }
        
        return identity;
    }
            
    // Try to retrieve the user principal stored in the authentication cookie and redirect
    // the user agent to the login page (or to an external provider) in the following cases:
    //  - If the user principal can't be extracted or the cookie is too old.
    //  - If a max_age parameter was provided and the authentication cookie is not considered "fresh" enough.
    //
    // For scenarios where the default authentication handler configured in the ASP.NET Core
    // authentication options shouldn't be used, a specific scheme can be specified here.
    private static bool IsAuthenticated(AuthenticateResult? result, OpenIddictRequest request)
    {
        if (result is { Succeeded: false }) { return false; }

        if (!request.MaxAge.HasValue || result?.Properties == null) return true;
        
        var maxAgeSeconds = TimeSpan.FromSeconds(request.MaxAge.Value);
        var expired = !result.Properties.IssuedUtc.HasValue 
                      || DateTimeOffset.UtcNow - result.Properties.IssuedUtc > maxAgeSeconds;
        return !expired;
    }

    private static string BuildRedirectUrl(HttpContext httpContext, List<KeyValuePair<string,StringValues>> parameters)
    {
        var url = $"{httpContext.Request.PathBase}{httpContext.Request.Path}{QueryString.Create(parameters)}";
        return url;
    }
    
    public static IDictionary<string, StringValues> ParseOAuthParameters(HttpContext context, List<string>? excluding = null)
    {
        excluding ??= [];
        var parameters = context.Request.HasFormContentType
            ? context.Request.Form
                .Where(parameter => !excluding.Contains(parameter.Key))
                .ToDictionary()
            : context.Request.Query
                .Where(parameter => !excluding.Contains(parameter.Key))
                .ToDictionary();
        
        return parameters;
    }

    
}