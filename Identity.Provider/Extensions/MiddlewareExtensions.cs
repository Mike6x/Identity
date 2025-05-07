using Identity.Infrastructure.Middleware;

namespace Identity.Provider.Extensions;

public static class MiddlewareExtensions
{
    public static IApplicationBuilder UseCustomExceptionMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ExceptionMiddleware>();
    }
}