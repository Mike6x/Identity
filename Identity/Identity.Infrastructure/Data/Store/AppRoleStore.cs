using System.Security.Claims;
using Identity.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.Data.Store;

public class AppRoleStore: RoleStore<AppRole, ApplicationDbContext, Guid, IdentityUserRole<Guid>, IdentityRoleClaim>
{
    private DbSet<IdentityRoleClaim> RoleClaims { get { return Context.Set<IdentityRoleClaim>(); } }

    public AppRoleStore(ApplicationDbContext context, IdentityErrorDescriber? describer = null)
        : base(context, describer)
    {
    }

    public override async Task<IList<Claim>> GetClaimsAsync(AppRole role, CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        if (role == null)
        {
            throw new ArgumentNullException(nameof(role));
        }

        return await RoleClaims.Where(rc => rc.RoleId.Equals(role.Id)).Select(c => c.ToClaim()).ToListAsync(cancellationToken);
    }

    protected override IdentityRoleClaim CreateRoleClaim(AppRole role, Claim claim)
    {
        var roleClaim = base.CreateRoleClaim(role, claim);
        roleClaim.InitializeFromClaim(claim);
        return roleClaim;
    }
}
