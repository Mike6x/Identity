using Identity.API.Configurations;
using Identity.API.EndPoints;
using Identity.API.EndPoints.Identity;
using Identity.API.EndPoints.OpenIDConnect;
using Identity.API.EndPoints.Users;

namespace Identity.API.Extensions;

public static class IdentityMiddlewarePipeline
{
    public static WebApplication UseAuthMiddlewarePipeline(this WebApplication app)
    {
        app.UseSwagger();
        app.UseUrlsFromConfig();
        app.UseReverseProxySupport();
        app.UseOpenIddict();

        app.UseAntiforgery();
        app.UseAuthentication();
        app.UseAuthorization();

        var apiEnpoints = app.MapGroup("api");
        apiEnpoints.MapStatusEndpoints();
        apiEnpoints.MapUsersEndpoints();
        apiEnpoints.MapIdentityEndpoints();

        app.MapOpenIdConnectEndpoints();
        app.MapExternalCallbackEndpoint();

        app.UseVueFallbackSpa();
        
        return app;
    }
}