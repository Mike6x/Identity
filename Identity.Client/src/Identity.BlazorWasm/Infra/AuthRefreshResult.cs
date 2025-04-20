using Identity.BlazorWasm.Infra;
using static Identity.BlazorWasm.Infra.AuthRefreshMessage;

namespace Identity.BlazorWasm.Infra;

public class AuthRefreshResult(AuthRefreshMessage resultInfo, string? message = null) 
{
    public string? AccessToken { get; }
    public string? RefreshToken { get; }
    public int ExpiresIn { get; set; }
    public bool Success => ResultInfo == Successful;
    public bool Failure => !Success;
    
    public string? Message { get; } = message;
    
    public AuthRefreshResult(string accessToken, string refreshToken, int expiresIn) : this(Successful)
    {
        AccessToken = accessToken;
        RefreshToken = refreshToken;
        ExpiresIn = expiresIn;
    }

    private AuthRefreshMessage ResultInfo { get; } = resultInfo;
    
    
}