using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Identity.Infrastructure.Data.Workers;

public sealed partial class OpenIdDictWorker(
    IServiceProvider serviceProvider, 
    IConfiguration configuration) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        _ = await context.Database.EnsureCreatedAsync(cancellationToken);
        
        await context
            .Database
            .MigrateAsync(cancellationToken: cancellationToken);
        
        await SeedScopesAsync(scope, cancellationToken);
        await SeedClientsAsync(scope, cancellationToken);
        await SeedRolesAsync(scope);
        await SeedUsersAsync(scope);
        
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    
}
