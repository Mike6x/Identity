// ReSharper disable InconsistentNaming
namespace Identity.UI.Infrastructure.Infra;

public class LoginResult 
{
    public string? access_token { get; set; }
    public string? refresh_token { get; set; }
    public string? token_type { get; set; }
    public int expires_in { get; set; }
}