using Identity.Core.Dtos;
using Identity.Core.Dtos.Accounts;
using Identity.Core.Dtos.Authentications;

namespace Identity.Core.Interfaces;

public interface IAuthenticationService
{
    Task<AuthenticationResult> LoginAsync(string email, string password);
    Task<AuthenticationResult> RegisterAsync(RegisterRequest request);
    Task<bool> LogoutAsync(string userId);
    Task<AuthenticationResult> RefreshTokenAsync(string refreshToken);
    Task<bool> RevokeTokenAsync(string token);
    Task<bool> ValidateTokenAsync(string token);
}
