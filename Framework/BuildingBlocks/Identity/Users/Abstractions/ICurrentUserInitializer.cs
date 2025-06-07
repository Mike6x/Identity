using System.Security.Claims;

namespace BuildingBlocks.Identity.Users.Abstractions;

public interface ICurrentUserInitializer
{
    void SetCurrentUser(ClaimsPrincipal user);

    void SetCurrentUserId(string userId);
}
