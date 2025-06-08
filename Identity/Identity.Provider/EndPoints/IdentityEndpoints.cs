using Identity.Provider.EndPoints.Authentication;
using Identity.Provider.EndPoints.Authenticator;
using Identity.Provider.EndPoints.Authorization;
using Identity.Provider.EndPoints.Claim;
using Identity.Provider.EndPoints.Client;
using Identity.Provider.EndPoints.CorsPolicy;
using Identity.Provider.EndPoints.ExternalCallback;
using Identity.Provider.EndPoints.ExternalLogin;
using Identity.Provider.EndPoints.permission;
using Identity.Provider.EndPoints.Role;
using Identity.Provider.EndPoints.Scope;
using Identity.Provider.EndPoints.Status;
using Identity.Provider.EndPoints.Users;

namespace Identity.Provider.EndPoints;

public static class IdentityEndpoints
{
    public static IApplicationBuilder MapIdentityEndpoints(this WebApplication app)
    {
        var statusGroup = app.MapGroup("status").WithTags("Status").WithName("Status Group").WithOpenApi();
        statusGroup.MapStatusEndpoints();
        
        var authorizationGroup = app.MapGroup("connect").WithTags("Authorization");
        authorizationGroup.MapOpenIdConnectEndpoints();
        
        var authGroup = app.MapGroup("api/auth").WithTags("Authentication").WithOpenApi();
        authGroup.MapAuthenticationEndpoints();
        
        var callbackGroup = app.MapGroup("api/callback").WithTags("ExternalCallback");
        callbackGroup.MapExternaCallbackEndpoints();
        
        var userGroup = app.MapGroup("users").WithTags("User").WithOpenApi();
        userGroup.MapUsersEndpoints();
        
        var scopeGroup = app.MapGroup("scopes").WithTags("Scope").WithOpenApi();
        scopeGroup.MapScopeEndpoints();

        
        // var userClaimGroup = app.MapGroup("users").WithTags("User Claims").WithOpenApi();
        // userClaimGroup.MapUserClaimEndpoints();
        //
        // var userRoleGroup = app.MapGroup("users").WithTags("User Roles").WithOpenApi();
        // userRoleGroup.MapUserRoleEndpoints();
        //
        // var accountGroup = app.MapGroup("accounts").WithTags("User Accounts").WithOpenApi();
        // accountGroup.MapAccountEndpoints();
        //
        // var currentUserGroup = app.MapGroup("users/Current").WithTags("Current Users").WithOpenApi();
        // currentUserGroup.MapCurrentUserEndpoints();
        
        var roleGroup = app.MapGroup("roles").WithTags("Roles").WithOpenApi();
        roleGroup.MapRoleEndpoints();
        
        var roleClaimGroup = app.MapGroup("roles").WithTags("Role Claims").WithOpenApi();
        roleClaimGroup.MapRoleClaimEndpoints();
        
        var rolePermissionGroup = app.MapGroup("roles").WithTags("Roles Permissions").WithOpenApi();
        rolePermissionGroup.MapRolePermissionEndpoints();
        
        
        var clientGroup = app.MapGroup("applications").WithTags("Client").WithOpenApi();
        clientGroup.MapApplicationEndpoints();
        
        var corsPolicyGroup = app.MapGroup("corspolicy") .WithTags("CorsPolicy").WithOpenApi();
        corsPolicyGroup.MapCorsPolicyEndpoints();
        
        var authenticatorGroup = app.MapGroup("Authenticator").WithTags("Authenticator").WithOpenApi();
        authenticatorGroup.MapAuthenticatorEndpoints();
        
        var externalLoginGroup = app.MapGroup("ExternalLogins").WithTags("ExternalLogin").WithOpenApi();
        externalLoginGroup.MapExternalLoginEndpoints();
        
        // var openIdConnectGroup = app.MapGroup("")
        // openIdConnectGroup.MapOpenIdDictEndpoints()
        
        return app;
    }
}