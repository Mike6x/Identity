using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;

namespace Identity.Core.Features.Authentication;

public interface IAuthService
{
    Task<IResult> LogInAsync(LoginRequest request);
    Task <IResult> LogOutAsync(string? returnUrl);
    
    Task<IResult> LogInCallBackAsync(HttpContext httpContext);
    
}