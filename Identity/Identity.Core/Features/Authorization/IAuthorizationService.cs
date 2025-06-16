using Microsoft.AspNetCore.Http;

namespace Identity.Core.Features.Authorization;

public interface IAuthorizationService
{
    Task<IResult> AuthorizeAsync(HttpContext httpContext);
    Task<IResult> AcceptAsync(HttpContext httpContext);
    IResult Deny();
    Task<IResult> VerifyAsync(HttpContext httpContext);
    Task<IResult> VerifyAcceptAsync(HttpContext httpContext);
    IResult VerifyDeny();
    Task<IResult> ExchangeAsync(HttpContext httpContext);

    Task<IResult> EndSessionAsync();

}