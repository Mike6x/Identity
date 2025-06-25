using Identity.Infrastructure.Middleware;

namespace Identity.AuthServer.Extensions;

public static class MiddlewareExtensions
{
    public static IApplicationBuilder UseCustomExceptionMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ExceptionMiddleware>();
    }
}