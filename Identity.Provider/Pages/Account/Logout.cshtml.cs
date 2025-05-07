using Identity.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Identity.Provider.Pages.Account;

public class Logout(SignInManager<AppUser> signInManager) : PageModel
{
    public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
    {
        await signInManager.SignOutAsync();

        if (returnUrl is not null) return Redirect(returnUrl);
        return RedirectToPage();
    }
}