using Identity.Infrastructure.Services.ExternalCallback;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Identity.Infrastructure.Endpoints.ExternalCallback;

public static class ExternalCallbackEndpoints
{
    public static IEndpointRouteBuilder MapExternaCallbackEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGetExternalCallbackEndpoint();
        app.MapExternalCallbackEndpoint();
        
        return app;
    }
}

//https://github.com/andyrub18/AuthServer/blob/main/AuthServer/Endpoints/ExternalCallbackEndpoint.cs

public static class GetExternalCallbackEndpoint
{
    public static RouteHandlerBuilder MapGetExternalCallbackEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return EndpointRouteBuilderExtensions.MapGet(endpoints, (string)"/signin-google", (Delegate)ExternalCallbackLoggin.Handler)
                .WithName(nameof(GetExternalCallbackEndpoint))
            ;
    }
}

public static class ExternalCallbackEndpoint
{
    public static RouteHandlerBuilder MapExternalCallbackEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return EndpointRouteBuilderExtensions.MapPost(endpoints, (string)"/signin-google", (Delegate)ExternalCallbackLoggin.Handler)
            .WithName(nameof(ExternalCallbackEndpoint));
    }
    
}