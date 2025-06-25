using BuildingBlocks.Identity.Users.Abstractions;
using FluentValidation;
using Identity.Core.Features.User.ResetPassword;
using Identity.Shared.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Identity.Infrastructure.Endpoints.UserAccount;

public static class ResetPasswordEndpoint
{
    internal static RouteHandlerBuilder MapResetPasswordEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPost("/reset-password", async (
            ResetPasswordCommand command, 
            [FromHeader(Name = TenantConstants.Identifier)] string tenant, 
            [FromServices] IValidator<ResetPasswordCommand> validator, 
            IUserService userService, 
            CancellationToken cancellationToken) =>
        {
            var result = await validator.ValidateAsync(command, cancellationToken);
            if (!result.IsValid)
            {
                return Results.ValidationProblem(result.ToDictionary());
            }

            await userService.ResetPasswordAsync(command, cancellationToken);
            return Results.Ok("Password has been reset.");
        })
        .WithName(nameof(ResetPasswordEndpoint))
        .WithSummary("Reset password")
        .WithDescription("Resets the password using the token and new password provided.")
        .AllowAnonymous();
    }

}
