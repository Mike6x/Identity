using BuildingBlocks.Identity.Users.Abstractions;
using Identity.Core.Features.User;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Provider.EndPoints.UserAccount
{
    public static class GetConirmEmailEndpoint
    {
        internal static RouteHandlerBuilder MapGetCornfirmEmailEndpoint(this IEndpointRouteBuilder endpoints)
        {
            return endpoints.MapGet("/confirm-email", async (
                [FromQuery] string userId,
                [FromQuery] string code,
                [FromQuery] string tenant,
                HttpContext context,
                // ITenantService tenantService,
                IUserService userService,
                CancellationToken cancellationToken) =>
            {
                // TenantDetail tenantDetail = await tenantService.GetByIdAsync(tenant) ?? throw new NotFoundException($"Tenant: {tenant} not found");
                // var tenantInfo = tenantDetail.Adapt<FshTenantInfo>();  
                // context.SetTenantInfo(tenantInfo, true);

                return Task.FromResult(userService.ConfirmEmailAsync(userId, code, tenant, cancellationToken));
            })
            .WithName(nameof(GetConirmEmailEndpoint))
            .WithSummary("Confirm email")
            .WithDescription("Confirm email address for a user.")
            .AllowAnonymous();
        }
        
    }
}
