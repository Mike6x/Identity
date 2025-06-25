using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Identity.AuthServer.Pages.Consent
{
    public class IndexModel : PageModel
    {
        [BindProperty] public string ApplicationName { get; set; } = "";
        [BindProperty] public string Scope { get; set; } = "";

        public void OnGet()
        {
            var jsonData = HttpContext.Session.GetString("ConsentData");
            if (jsonData == null) return;
            HttpContext.Session.Clear();
            var jsonObject = JsonSerializer.Deserialize<JsonNode>(jsonData);
            if (jsonObject == null) return;
            ApplicationName = jsonObject["applicationName"]?.GetValue<string?>()?? string.Empty;
            Scope = jsonObject["scope"]?.GetValue<string?>()?? string.Empty;
        }

        public ActionResult OnPost()
        {
            return RedirectToPage("/connect/authorize");
        }
    }
}