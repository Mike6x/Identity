using Identity.API.Configurations;
using Identity.API.EndPoints;
using Identity.API.EndPoints.Authentication;
using Identity.API.EndPoints.Authorization;
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

        var callbackGroup = app.MapGroup("api/callback").WithTags("External Logins");
        callbackGroup.MapExternalCallbackEndpoints();
        
        var authGroup = app.MapGroup("api/auth").WithTags("Authentications");
        authGroup.MapIdentityEndpoints();
        
        var userGroup = app.MapGroup("api/users").WithTags("Users");
        userGroup.MapUsersEndpoints();
        
        var statusGroup = app.MapGroup("api/status").WithTags("Status");
        statusGroup.MapStatusEndpoints();
        
        var authorizationGroup = app.MapGroup("connect").WithTags("Authorizations");
        authorizationGroup.MapOpenIdConnectEndpoints();
        

        app.UseVueFallbackSpa();
        
        return app;
    }
}