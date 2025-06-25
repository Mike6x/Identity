using Identity.Core.Features.Client;
using Identity.Core.Features.Scope;
using Identity.Infrastructure.Data.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace Identity.AuthServer.Pages.Admin.Clients;

public class AddClient(
    ClientAppRepository clientAppRepository,
    ScopesRepository scopesRepository
) : PageModel
{
    [BindProperty] public ClientDto InputModel { get; set; } = null!;

    public MultiSelectList? AvailableScopes { get; set; }

    public bool Created { get; set; }

    public IActionResult OnGet()
    {
        PopulateScopes();
        return Page();
    }

    public IActionResult OnPost()
    {
        if (!ModelState.IsValid) return Page();

        var res = clientAppRepository.CreateClient(InputModel);

        if (res) Created = true;

        return Page();
    }

    private void PopulateScopes()
    {
        var scopes = scopesRepository.GetScopes().Result;
        AvailableScopes = new(scopes, nameof(ScopeSummaryDto.Name), nameof(ScopeSummaryDto.Name));
    }
}