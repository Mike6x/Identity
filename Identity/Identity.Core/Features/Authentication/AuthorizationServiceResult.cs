namespace Identity.Core.Features.Authentication;

public class AuthorizationServiceResult
{
    public string? ApplicationName { get; set; } = string.Empty;
    public string? Scope { get; set; } = string.Empty;
}
