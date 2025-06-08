namespace Identity.Provider.EndPoints.UserAccount;

public static class AccountEndpoints
{
    public static IEndpointRouteBuilder MapAccountEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapRegisterUserEndpoint();
        
        app.MapChangePasswordEndpoint();
        app.MapForgotPasswordEndpoint();
        app.MapResetPasswordEndpoint();
        
        app.MapGetCornfirmEmailEndpoint();
        app.MapGetCornfirmPhoneNumberEndpoint();
        app.MapCornfirmEmailEndpoint();
        app.MapSendVerificationEmailEndPoint();

        app.MapHasPasswordEndpoint();
        app.MapDeleteAccountEndpoint();
        
        return app;
    }
}