using Identity.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;

namespace Identity.API.EndPoints.Identity;

public static class IdentityEndpoints
{
    public static IEndpointRouteBuilder MapIdentityEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("");

        group
            .WithTags("Identity")
            .WithName("Identity");

        group.MapPost("/login", LoginHandler)
            .WithName("Login")
            .AllowAnonymous();

        group.MapPost("/logout", LogoutHandler)
            .WithName("Logout")
            .RequireAuthorization();


        #region Handlers


        async Task<IResult> LoginHandler(
                       LoginRequest dto,

                       SignInManager<ApplicationUser> signInManager,
                       UserManager<ApplicationUser> userManager,
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

        async Task LogoutHandler(SignInManager<ApplicationUser> signInManager)
        {
            await signInManager.SignOutAsync();
        }

        #endregion

        return app;
    }


}
