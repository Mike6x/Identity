using Identity.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Identity.Provider.Configurations;

public static class DatabaseConfig
{
    public static IServiceCollection AddDatabaseConfig(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection") ??
                               throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseSqlServer(connectionString);
            
            // options.UseSqlite(connectionString);
            
            // options.UseNpgsql(connectionString);
            // options.ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning));

        });
        services.AddDatabaseDeveloperPageExceptionFilter();
        
        return services;
    }
}