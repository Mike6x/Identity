using Identity.Core.Features.CorsPolicy;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Http;

namespace Identity.Infrastructure.Services.CorsPolicy;

/// <summary>
/// Add specified origins to the list of allowed origins on default cors policy.
/// </summary>
/// <returns></returns>
public static class AddOrigins
{
    public static async Task<List<string>>  Handler(
        HttpContext httpContext,
        ICorsPolicyProvider corsPolicyProvider,
        AddOrRemoveOriginsCommand request,
        CancellationToken cancellationToken)
    {
        var defaultCorsPolicy = await corsPolicyProvider.GetPolicyAsync(httpContext, null) 
                                ?? new Microsoft.AspNetCore.Cors.Infrastructure.CorsPolicy();

        foreach (var uri in request.Origins.Select(s => new Uri(s)))
        {
            var origin = $"{uri.Scheme}://{uri.Authority}";
            if (!defaultCorsPolicy.Origins.Contains(origin))
            {
                defaultCorsPolicy.Origins.Add(origin);
            }
        }

        return defaultCorsPolicy.Origins.ToList();
    }
}