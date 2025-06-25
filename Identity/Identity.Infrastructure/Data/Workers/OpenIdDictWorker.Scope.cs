using Microsoft.Extensions.DependencyInjection;
using OpenIddict.Abstractions;

namespace Identity.Infrastructure.Data.Workers;

public partial class OpenIdDictWorker
{
    private static async Task SeedScopesAsync(IServiceScope scope, CancellationToken cancellationToken)
    {
        var scopeManager = scope.ServiceProvider.GetRequiredService<IOpenIddictScopeManager>();
        
        var seedingList = new ScopeCollection().GetAll().ToList();
        
        if (await scopeManager.CountAsync(cancellationToken) == 0 && seedingList.Count != 0)
        {
            foreach (var item in seedingList)
            {
                await scopeManager.CreateAsync(item, cancellationToken);
            }
        }
    }
}