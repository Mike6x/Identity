using Identity.Provider.EndPoints.User.BasicFeatures;
using Identity.Provider.EndPoints.User.ManagementExtensions;
using Identity.Provider.EndPoints.User.Others;

namespace Identity.Provider.EndPoints.User;

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