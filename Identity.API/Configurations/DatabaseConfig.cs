using Identity.API.Data;
using Microsoft.EntityFrameworkCore;

namespace Identity.API.Configurations;

public static class DatabaseConfig
{
    public static IServiceCollection AddDatabaseConfig(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection") ??
                               throw new InvalidOperationException("Connection string not found.");

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