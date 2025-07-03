using Identity.Core.Models;
using Microsoft.AspNetCore.Http;

namespace Identity.Core.Features.Authenticator;

public interface IAuthenticatorService
{
    Task<bool> IsAuthenticatorEnabledAsync(HttpContext context);
    Task<EnableAuthenticatorModel> RetrieveAuthenticatorConfigAsync(HttpContext context);
    Task<IEnumerable<string>?> GenerateRecoveryCodesAsync(HttpContext context);
    Task<int> CountActiveRecoveryCodesAsync(HttpContext context);
    Task<IResult> EnableAuthenticatorAsync(string code, HttpContext context);
    Task<IResult> DisableAuthenticatorAsync(string code, HttpContext context);
    Task<IResult> ResetAuthenticatorAsync(string code, HttpContext context);
}