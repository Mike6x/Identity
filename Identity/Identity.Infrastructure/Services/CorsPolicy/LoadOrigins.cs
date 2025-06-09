using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Identity.Infrastructure.Services.CorsPolicy;

public static class LoadOrigins
{
    public static Task<List<string>>  Handler (
        HttpContext httpContext,
        IConfiguration configuration)
    {
        var allowedOrigins = configuration.GetSection("CorsOptions:AllowedOrigins").Get<string[]>() ?? [];
        
        var defaultCorsPolicy = new Microsoft.AspNetCore.Cors.Infrastructure.CorsPolicy();
        
        foreach (var origin in allowedOrigins)
        {
            defaultCorsPolicy.Origins.Add(origin);
        }
        
        return Task.FromResult(defaultCorsPolicy.Origins.ToList());
    }
}

public static class LoadOrigins1
{
    public static Task<List<string>>  Handler (IOptions<CorsOptions> corsOptions)
    {
                
        var defaultCorsPolicy = corsOptions.Value.GetPolicy(corsOptions.Value.DefaultPolicyName)
                                ?? new Microsoft.AspNetCore.Cors.Infrastructure.CorsPolicy();
                
        return Task.FromResult(defaultCorsPolicy.Origins.ToList());
    }
}