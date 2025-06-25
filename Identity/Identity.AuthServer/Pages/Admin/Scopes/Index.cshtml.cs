using Identity.Core.Features.Scope;
using Identity.Infrastructure.Data.Repository;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Identity.AuthServer.Pages.Admin.Scopes;

public class IndexModel(ScopesRepository repository) : PageModel
{
    public IEnumerable<ScopeSummaryDto> Scopes { get; private set; } = null!;
    public string? Filter { get; set; }

    public async Task OnGetAsync(string? filter)
    {
        Filter = filter;
        Scopes = await repository.GetScopes();
    }
}