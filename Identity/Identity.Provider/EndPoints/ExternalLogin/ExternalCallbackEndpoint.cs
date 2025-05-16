namespace Identity.Provider.EndPoints.ExternalLogin;

public static class Extensions
{
    public static IEndpointRouteBuilder MapExternalCallbackEndpoints(this IEndpointRouteBuilder app)
    {
 
        app.MapGetExternalCallbackEndpoint();
        app.MapExternalCallbackEndpoint();
        
        return app;
    }
}

public static class GetExternalCallbackEndpoint
{
    public static RouteHandlerBuilder MapGetExternalCallbackEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return EndpointRouteBuilderExtensions.MapGet(endpoints, (string)"/signin-google", (Delegate)ExternalLogin.Handler)
                .WithName(nameof(GetExternalCallbackEndpoint))
            ;
    }
}

public static class ExternalCallbackEndpoint
{
    public static RouteHandlerBuilder MapExternalCallbackEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return EndpointRouteBuilderExtensions.MapPost(endpoints, (string)"/signin-google", (Delegate)ExternalLogin.Handler)
            .WithName(nameof(ExternalCallbackEndpoint));
    }
    
}