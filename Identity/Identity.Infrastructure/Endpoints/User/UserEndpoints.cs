using Identity.Infrastructure.Endpoints.User.BasicFeatures;
using Identity.Infrastructure.Endpoints.User.ManagementExtensions;
using Identity.Infrastructure.Endpoints.User.Others;
using Microsoft.AspNetCore.Routing;

namespace Identity.Infrastructure.Endpoints.User;

public static class UserEndpoints
{
    public static IEndpointRouteBuilder MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapCreateUserEndpoint();
        app.MapGetUsersEndpoint();
        app.MapGetUserEndpoint();
        app.MapSearchUsersEndpoint();
        app.MapDeleteUserEndpoint();
        app.MapUpdateUserEndpoint();
        
        app.MapExportUsersEndpoint();
        app.MapImportUsersEndpoint();
        
        app.MapLockUserEndpoint();
        app.MapUnLockUserEndpoint();
        app.MapToggleUserStatusEndpoint();
        app.MapSetOnlineStatusEndpoint();
        // app.MapDisableUserEndpoint();
        app.MapToggleOnlineStatusEndpoint();
        
        app.MapGetUserByEmailEndpoint();
        app.MapGetUserByNameEndpoint();
        app.MapGetUserByPhoneNumberEndpoint();
        app.MapGetOtherUsersEndpoint();
        
        app.MapGetUserStatisticsEndpoint();
        //app.MapGetUserAuditTrailEndpoint()
        
        return app;
    }
}