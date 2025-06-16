using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using OpenIddict.Abstractions;

namespace Identity.Infrastructure.Services.Authorization;

public static class AuthorizationServiceHelpers
{
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
    
    public static string BuildRedirectUrl(HttpRequest request, IDictionary<string, StringValues> parameters)
    {
        var url = $"{request.PathBase}{request.Path}{QueryString.Create(parameters)}";
        return url;
    }
    
    public static bool IsAuthenticated(AuthenticateResult? result, OpenIddictRequest request)
    {
        if (result is { Succeeded: false }) { return false; }

        if (!request.MaxAge.HasValue || result?.Properties == null) return true;
        
        var maxAgeSeconds = TimeSpan.FromSeconds(request.MaxAge.Value);
        var expired = !result.Properties.IssuedUtc.HasValue 
                      || DateTimeOffset.UtcNow - result.Properties.IssuedUtc > maxAgeSeconds;
        return !expired;
    }

    public static List<string> GetDestinations(ClaimsIdentity identity, Claim claim)
    {
        var destinations = new List<string>();

        if (claim.Type is OpenIddictConstants.Claims.Name 
            or OpenIddictConstants.Claims.Email)
        {
            destinations.Add(OpenIddictConstants.Destinations.AccessToken);

            if (identity.HasScope(OpenIddictConstants.Scopes.OpenId))
            {
                destinations.Add(OpenIddictConstants.Destinations.IdentityToken);
            }
        }

        return destinations;
    }
    
}