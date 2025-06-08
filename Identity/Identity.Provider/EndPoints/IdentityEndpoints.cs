using Identity.Provider.EndPoints.Authentication;
using Identity.Provider.EndPoints.Authenticator;
using Identity.Provider.EndPoints.Authorization;
using Identity.Provider.EndPoints.Claim;
using Identity.Provider.EndPoints.Client;
using Identity.Provider.EndPoints.CorsPolicy;
using Identity.Provider.EndPoints.CurrentUser;
using Identity.Provider.EndPoints.ExternalCallback;
using Identity.Provider.EndPoints.ExternalLogin;
using Identity.Provider.EndPoints.permission;
using Identity.Provider.EndPoints.Role;
using Identity.Provider.EndPoints.Scope;
using Identity.Provider.EndPoints.Status;
using Identity.Provider.EndPoints.User;
using Identity.Provider.EndPoints.UserAccount;
using Identity.Provider.EndPoints.UserClaim;
using Identity.Provider.EndPoints.UserRole;

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
        
        var accountGroup = app.MapGroup("accounts").WithTags("Accounts").WithOpenApi();
        accountGroup.MapAccountEndpoints();
        
        var currentUserGroup = app.MapGroup("users/Current").WithTags("Current User").WithOpenApi();
        currentUserGroup.MapCurrentUserEndpoints();
        
        
        var userGroup = app.MapGroup("users").WithTags("Users").WithOpenApi();
        userGroup.MapUserEndpoints();
        
        var userClaimGroup = app.MapGroup("users").WithTags("User Claims").WithOpenApi();
        userClaimGroup.MapUserClaimEndpoints();
        
        var userRoleGroup = app.MapGroup("users").WithTags("User Roles").WithOpenApi();
        userRoleGroup.MapUserRoleEndpoints();
        
        
        var roleGroup = app.MapGroup("roles").WithTags("Roles").WithOpenApi();
        roleGroup.MapRoleEndpoints();
        
        var roleClaimGroup = app.MapGroup("roles").WithTags("Role Claims").WithOpenApi();
        roleClaimGroup.MapRoleClaimEndpoints();
        
        var rolePermissionGroup = app.MapGroup("roles").WithTags("Role Permissions").WithOpenApi();
        rolePermissionGroup.MapRolePermissionEndpoints();
        
        
        var scopeGroup = app.MapGroup("scopes").WithTags("Scopes").WithOpenApi();
        scopeGroup.MapScopeEndpoints();
        
        var clientGroup = app.MapGroup("applications").WithTags("Applications").WithOpenApi();
        clientGroup.MapApplicationEndpoints();
        
        var corsPolicyGroup = app.MapGroup("corspolicy") .WithTags("CorsPolicy").WithOpenApi();
        corsPolicyGroup.MapCorsPolicyEndpoints();
        
        var authenticatorGroup = app.MapGroup("Authenticator").WithTags("Authenticators").WithOpenApi();
        authenticatorGroup.MapAuthenticatorEndpoints();
        
        var externalLoginGroup = app.MapGroup("ExternalLogins").WithTags("ExternalLogin").WithOpenApi();
        externalLoginGroup.MapExternalLoginEndpoints();
        
        var callbackGroup = app.MapGroup("api/callback").WithTags("ExternalCallback").WithOpenApi();
        callbackGroup.MapExternaCallbackEndpoints();
        
        // var openIdConnectGroup = app.MapGroup("")
        // openIdConnectGroup.MapOpenIdDictEndpoints()
        
        return app;
    }
}