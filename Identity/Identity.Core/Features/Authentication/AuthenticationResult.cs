namespace Identity.Core.Features.Authentication;

public class AuthenticationResult
{
    public bool Succeeded { get; set; }
    public string? Token { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public IEnumerable<string>? Errors { get; set; }
}
