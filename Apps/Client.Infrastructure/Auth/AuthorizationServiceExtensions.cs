using System.Security.Claims;
using Identity.Shared.Authorization;
using Microsoft.AspNetCore.Authorization;

namespace Client.Infrastructure.Auth;

public static class AuthorizationServiceExtensions
{
    public static async Task<bool> HasPermissionAsync(this IAuthorizationService service, ClaimsPrincipal user, string action, string resource)
    {
        return (await service.AuthorizeAsync(user, null, AppPermission.NameFor(action, resource))).Succeeded;
    }
}
