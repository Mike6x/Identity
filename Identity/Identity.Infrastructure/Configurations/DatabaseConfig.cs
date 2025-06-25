using Identity.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Infrastructure.Configurations;

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
            // options.UseOpenIddict();
        });
        services.AddDatabaseDeveloperPageExceptionFilter();
        
        return services;
    }
}