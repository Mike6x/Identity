using Identity.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;

namespace Identity.Provider.EndPoints.Authentication;

public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapIdentityEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapLogInEndpoint1();
        app.MapLogOutEndpoint1();

        return app;
    }


}

public static class LogInEndpoint1
{
    public static RouteHandlerBuilder MapLogInEndpoint1(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPost("/login",
                LoginHandler)
            .WithName(nameof(LogInEndpoint1))
            .WithSummary("Login User")
            .WithDescription("Login User")
            .AllowAnonymous();
    }
    
    private static async Task<IResult> LoginHandler(
        LoginRequest dto,

        SignInManager<AppUser> signInManager,
        UserManager<AppUser> userManager,
        HttpContext httpContext)
    {
        var user = await userManager.FindByEmailAsync(dto.Email);

        var isPersistent = true;

        if (string.IsNullOrWhiteSpace(user?.UserName))
        {
            await Task.Delay(Random.Shared.Next(100, 500));
            return Results.Unauthorized();
        }

        var result = await signInManager.PasswordSignInAsync(user.UserName, dto.Password, isPersistent, false);

        if (result.RequiresTwoFactor)
        {
            if (!string.IsNullOrEmpty(dto.TwoFactorCode))
            {
                result = await signInManager.TwoFactorAuthenticatorSignInAsync(dto.TwoFactorCode, isPersistent, rememberClient: isPersistent);
            }
            else if (!string.IsNullOrEmpty(dto.TwoFactorRecoveryCode))
            {
                result = await signInManager.TwoFactorRecoveryCodeSignInAsync(dto.TwoFactorRecoveryCode);
            }
            if (!result.Succeeded)
                return Results.Accepted("Otp Required");
        }

        //var user = await userManager.FindByEmailAsync("admin@localhost");

        //await signInManager.SignInAsync(user, true);
        if (!result.Succeeded)
            return Results.Unauthorized();

        return Results.Ok("Logged In");
    }

}

public static class LogOutEndpoint1
{
    public static RouteHandlerBuilder MapLogOutEndpoint1(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPost("/logout", LogoutHandler)
            .WithName(nameof(LogOutEndpoint1))
            .WithSummary("Log Out")
            .WithDescription("Log Out.")
            .RequireAuthorization();
    }
    
    private static async Task LogoutHandler(SignInManager<AppUser> signInManager)
    {
        await signInManager.SignOutAsync();
    }

}
