using FluentValidation;
using FluentValidation.Results;
using Identity.Core.Features.User;
using Identity.Core.Features.User.EmailConfirm;
using Identity.Shared.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Provider.EndPoints.UserAccount
{
    public static class ConirmEmailEndpoint
    {
        internal static RouteHandlerBuilder MapCornfirmEmailEndpoint(this IEndpointRouteBuilder endpoints)
        {
            return endpoints.MapPost("/confirm-email", async (
                EmailConfirmCommand command, 
                [FromHeader(Name = TenantConstants.Identifier)] string tenant, 
                [FromServices] IValidator<EmailConfirmCommand> validator, 
                IUserService userService, 
                CancellationToken cancellationToken) =>
            {
            
                ValidationResult result = await validator.ValidateAsync(command, cancellationToken);
                if (!result.IsValid)
                {
                    return Results.ValidationProblem(result.ToDictionary());
                }

                await userService.ConfirmEmailAsync(command.UserId, command.Code, command.Tenant, cancellationToken);
                
                return Results.Ok("Email Confirmed.");

            })
            .WithName(nameof(ConirmEmailEndpoint))
            .WithSummary("Confirm email")
            .WithDescription("Confirm email address for a user.")
            .AllowAnonymous();
        }
    }
}
