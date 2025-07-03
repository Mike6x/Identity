using Identity.Core.Features.Authenticator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Identity.Infrastructure.Endpoints.Authenticator;

public static class AuthenticatorEndpoints
{
    public static IEndpointRouteBuilder MapAuthenticatorEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapIsAuthenticatorEnabledEndpoint();
        app.MapGetAuthenticatorConfigEndpoint();
        app.MapGenerateRecoverCodesEndpoint();
        app.MapCountActiveRecoveryCodesEndpoint();
        
        app.MapEnableAuthenticatorEndpoint();
        app.MapDisableAuthenticatorEndpoint();
        app.MapResetAuthenticatorEndpoint();
        
        return app;
    }
}

public static class IsAuthenticatorEnabledEndpoint
{
    public static RouteHandlerBuilder MapIsAuthenticatorEnabledEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapGet("/isenabled", async ( IAuthenticatorService service, HttpContext context) 
                    => await service.IsAuthenticatorEnabledAsync(context))
            .WithName(nameof(IsAuthenticatorEnabledEndpoint))
            .WithSummary("Is enabled")
            .WithDescription("Check whether authenticator is enabled for the user account");
    }
}

public static class GetAuthenticatorConfigEndpoint
{
    public static RouteHandlerBuilder MapGetAuthenticatorConfigEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapGet("/", async (IAuthenticatorService service, HttpContext context) 
                => await service.RetrieveAuthenticatorConfigAsync(context))
            .WithName(nameof(GetAuthenticatorConfigEndpoint))
            .WithSummary("Get User Authenticator And Uri")
            .WithDescription("Get the details required for setting up authenticator for the user");
    }
}

public static class GenerateRecoverCodesEndpoint
{
    public static RouteHandlerBuilder MapGenerateRecoverCodesEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapGet("/recoverycodes", async (IAuthenticatorService service, HttpContext context) 
                => await service.GenerateRecoveryCodesAsync(context))
            .WithName(nameof(GenerateRecoverCodesEndpoint))
            .WithSummary("Generate new recovery codes")
            .WithDescription("Generate new recovery codes for the authenticator");
    }
}

public static class CountActiveRecoveryCodesEndpoint
{
    public static RouteHandlerBuilder MapCountActiveRecoveryCodesEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapGet("/recoverycodescount", async (IAuthenticatorService service, HttpContext context) 
                => await service.CountActiveRecoveryCodesAsync(context))
            .WithName(nameof(CountActiveRecoveryCodesEndpoint))
            .WithSummary("Count Active recovery codes")
            .WithDescription(" Get the number of remaining recovery codes ");
    }
}

public static class EnableAuthenticatorEndpoint
{
    public static RouteHandlerBuilder MapEnableAuthenticatorEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPost("/enable", async (string code, IAuthenticatorService service, HttpContext context) 
                => await service.EnableAuthenticatorAsync(code, context))
                .WithName(nameof(EnableAuthenticatorEndpoint))
                .WithSummary("Enable the authenticator or user account")
                .WithDescription("Enable the authenticator by verifying the provided Code from authenticator app.");
    }
}

public static class DisableAuthenticatorEndpoint
{
    public static RouteHandlerBuilder MapDisableAuthenticatorEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPost("/disable", async (string code, IAuthenticatorService service, HttpContext context) 
                => await service.DisableAuthenticatorAsync(code, context))
                .WithName(nameof(DisableAuthenticatorEndpoint))
                .WithSummary("Disable authenticator for the user account")
                .WithDescription("Disable authenticator for the user account.");
    }
}

public static class ResetAuthenticatorEndpoint
{
    public static RouteHandlerBuilder MapResetAuthenticatorEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPost("/reset", async (string code, IAuthenticatorService service, HttpContext context) 
                => await service.ResetAuthenticatorAsync(code, context))
            .WithName(nameof(ResetAuthenticatorEndpoint))
            .WithSummary("Reset authenticator for the user account")
            .WithDescription("Reset authenticator for the user account.");
    }
}
