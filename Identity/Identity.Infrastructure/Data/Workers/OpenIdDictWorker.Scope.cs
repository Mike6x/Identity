using Microsoft.Extensions.DependencyInjection;
using OpenIddict.Abstractions;

namespace Identity.Infrastructure.Data.Workers;

public partial class OpenIdDictWorker
{
    private static async Task SeedScopesAsync(IServiceScope scope, CancellationToken cancellationToken)
    {
        var scopesManager = scope.ServiceProvider.GetRequiredService<IOpenIddictScopeManager>();
        
        var seedingList = new ScopeCollection().GetAll().ToList();
        
        if (await scopesManager.CountAsync(cancellationToken) == 0 && seedingList.Count != 0)
        {
            foreach (var item in seedingList)
            {
                await scopesManager.CreateAsync(item, cancellationToken);
            }
        }
    }
}