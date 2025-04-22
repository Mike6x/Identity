// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable InconsistentNaming
namespace Identity.UI.Infrastructure.Infra;

public class RefreshResult 
{
    public string? access_token { get; set; }
    public string? refresh_token { get; set; }
    public string? token_type { get; set; }
    public int expires_in { get; set; }
    public string? scope { get; set; }
    }