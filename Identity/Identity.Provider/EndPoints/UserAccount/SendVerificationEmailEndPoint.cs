using Identity.Core.Features.User;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Provider.EndPoints.UserAccount
{
    public static class SendVerificationEmailEndPoint
    {
        internal static RouteHandlerBuilder MapSendVerificationEmailEndPoint(this IEndpointRouteBuilder endpoints)
        {
            return endpoints.MapPost("/{userId}/verification-email", async (
                string userId,
                HttpContext context,
                [FromServices] IUserService userService,
                CancellationToken cancellationToken) =>
                {
                    // using with get endpoint
                    // var origin = $"{context.Request.Scheme}://{context.Request.Host.Value}{context.Request.PathBase.Value}"

                    var originUrl = context.Request.Headers.Origin;

                    await userService.SendVerificationEmailAsync(userId, originUrl!, cancellationToken);
                    return Results.Ok();
                })
                .WithName(nameof(SendVerificationEmailEndPoint))
                .WithSummary("Send email to verify user")
                // .RequirePermission("Permissions.Handlers.Update")
                .WithDescription("Send email to verify user");
        }
    }
}
