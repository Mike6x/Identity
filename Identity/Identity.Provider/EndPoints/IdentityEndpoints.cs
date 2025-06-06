using Identity.Provider.EndPoints.Authentication;
using Identity.Provider.EndPoints.Authorization;
using Identity.Provider.EndPoints.ExternalLogin;
using Identity.Provider.EndPoints.Status;
using Identity.Provider.EndPoints.Users;

namespace Identity.Provider.EndPoints;

public static class IdentityEndpoints
{
    public static IApplicationBuilder MapIdentityEndpoints(this WebApplication app)
    {
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
        
        return app;
    }
}