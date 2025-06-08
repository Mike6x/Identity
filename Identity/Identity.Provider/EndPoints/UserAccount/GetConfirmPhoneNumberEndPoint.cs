using Identity.Core.Features.User;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Provider.EndPoints.UserAccount
{

    public static class GetConfirmPhoneNumberEndpoint
    {
        internal static RouteHandlerBuilder MapGetCornfirmPhoneNumberEndpoint(this IEndpointRouteBuilder endpoints)
        {
            return endpoints.MapGet("/confirm-phone-number", (
                [FromQuery] string userId,
                [FromQuery] string code,
                IUserService userService,
                CancellationToken cancellationToken) =>
            {

                return Task.FromResult(userService.ConfirmPhoneNumberAsync(userId, code, cancellationToken));
            })
            .WithName(nameof(GetConfirmPhoneNumberEndpoint))
            .WithSummary("Confirm phone number")
            .WithDescription("Confirm phone number for a user.")
            .AllowAnonymous();
        }
        
    }
}
