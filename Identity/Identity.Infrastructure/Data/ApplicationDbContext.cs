using Identity.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.Data
{
    public class ApplicationDbContext(DbContextOptions options) :
        IdentityDbContext<
            AppUser,
            AppRole,
            Guid,
            IdentityUserClaim,
            IdentityUserRole<Guid>,
            IdentityUserLogin<Guid>,
            IdentityRoleClaim,
            IdentityUserToken<Guid>>(options)
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseOpenIddict();
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(GetType().Assembly);
        }
    }
    
    
    // public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
    //     : IdentityDbContext<AppUser, AppRole, Guid>(options)
    // {
    //     protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //     {
    //         base.OnConfiguring(optionsBuilder);
    //         optionsBuilder.UseOpenIddict();
    //     }
    // }
}