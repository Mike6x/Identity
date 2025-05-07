namespace Identity.Core.Settings;

public class ApplicationConfig
{
    public string DisplayName { get; set; }
    public string ClientId { get; set; }
    public string ClientSecret { get; set; }
    public string Scope { get; set; }
    public IEnumerable<string> RedirectUri { get; set; }
    public IEnumerable<string> PostLogoutRedirectUri { get; set; }
    public bool PKCE { get; set; }
}
