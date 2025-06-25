// using System.Text.Json;
// using System.Text.Json.Nodes;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.AspNetCore.Mvc.RazorPages;
//
// namespace Identity.AuthServer.Areas.Consent.Pages
// {
//     public class IndexModel : PageModel
//     {
//         public IndexModel()
//         {
//         }
//
//         [BindProperty]
//         public string ApplicationName { get; set; }
//         [BindProperty]
//         public string Scope { get; set; }
//
//         public void OnGet()
//         {
//             string jsonData = HttpContext.Session.GetString("ConsentData")!;
//             HttpContext.Session.Clear();
//             JsonNode jsonObject = JsonSerializer.Deserialize<JsonNode>(jsonData)!;
//             ApplicationName = jsonObject["applicationName"]?.GetValue<string?>()!;
//             Scope = jsonObject["scope"]?.GetValue<string?>()!;
//         }
//
//         public ActionResult OnPost()
//         {
//             return RedirectToPage("/connect/authorize");
//         }
//     }
// }