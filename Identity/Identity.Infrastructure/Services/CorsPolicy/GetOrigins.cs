using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Http;

namespace Identity.Infrastructure.Services.CorsPolicy;

public static class GetOrigins
{
    public static async Task<List<string>>  Handler(
        HttpContext httpContext,
        ICorsPolicyProvider corsPolicyProvider,
        CancellationToken cancellationToken)
            {
                var defaultCorsPolicy = await corsPolicyProvider.GetPolicyAsync(httpContext, null);
                
                return defaultCorsPolicy is null ? [] : defaultCorsPolicy.Origins.ToList();
            }
}


