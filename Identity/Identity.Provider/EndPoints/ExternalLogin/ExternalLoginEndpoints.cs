using Identity.Infrastructure.Services.ExternalLogins;

namespace Identity.Provider.EndPoints.ExternalLogin;

public static class ExternalLoginEndpoints
{
    public static IEndpointRouteBuilder MapExternalLoginEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGetExternalLoginsEndpoint();
        app.MapDeleteExternalLoginEndpoint();
        
        return app;
    }
}

public static class GetExternalLoginsEndpoint
{
    public static RouteHandlerBuilder MapGetExternalLoginsEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapGet("/", GetExternalLogins.Handler)
            .WithName(nameof(GetExternalLoginsEndpoint))
            .RequireAuthorization()
            .WithSummary("Get all external logins.")
            .WithDescription("Get all external logins for user account");
    }
}

public static class DeleteExternalLoginEndpoint
{
    public static RouteHandlerBuilder MapDeleteExternalLoginEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapDelete("/{loginProvider}/{providerKey}", DeleteExternalLogin.Handler)
            .WithName(nameof(DeleteExternalLoginEndpoint))
            .WithSummary("Remove external login.")
            .RequireAuthorization()
            // .RequirePermission("Permissions.Handlers.View")
            .WithDescription("Remove external login from user account.");
    }
}
