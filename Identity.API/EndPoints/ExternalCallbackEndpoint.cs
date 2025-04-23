namespace Identity.API.EndPoints;

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
        return endpoints.MapGet("/signin-google", ExternalLogin.Handler)
                .WithName(nameof(GetExternalCallbackEndpoint))
            ;
    }
}

public static class ExternalCallbackEndpoint
{
    public static RouteHandlerBuilder MapExternalCallbackEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPost("/signin-google", ExternalLogin.Handler)
            .WithName(nameof(ExternalCallbackEndpoint));
    }
    
}