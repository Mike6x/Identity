using System.Security.Claims;
using Identity.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Identity.Infrastructure.Data.Store;

public class AppUserStore : UserStore<AppUser, AppRole, ApplicationDbContext, Guid,
    IdentityUserClaim, IdentityUserRole<Guid>, IdentityUserLogin<Guid>,
    IdentityUserToken<Guid>, IdentityRoleClaim>
{
    public AppUserStore(ApplicationDbContext context, IdentityErrorDescriber describer = null)
        : base(context, describer)
    {
    }

    public override Task<IList<Claim>> GetClaimsAsync(AppUser user, CancellationToken cancellationToken = default)
    {
        return base.GetClaimsAsync(user, cancellationToken);
    }

    protected override IdentityUserClaim CreateUserClaim(AppUser user, Claim claim)
    {
        var userClaim = base.CreateUserClaim(user, claim);
        userClaim.InitializeFromClaim(claim);
        return userClaim;
    }
}