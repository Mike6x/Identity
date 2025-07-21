using System.Security.Claims;

namespace Identity.Shared.Authorization;

public static class ClaimsPrincipalExtensions
{
    public static string GetTenant(this ClaimsPrincipal principal)
        => principal.FindFirst(AppClaims.Tenant)?.Value ?? string.Empty;
    public static string GetUserId(this ClaimsPrincipal principal)
        => principal.FindFirst(AppClaims.UserId)?.Value ?? string.Empty;
    public static string GetEmail(this ClaimsPrincipal principal)
        => principal.FindFirst(AppClaims.Email)?.Value ?? string.Empty;
    
    public static string GetUserName(this ClaimsPrincipal principal)
        => principal.FindFirst(AppClaims.UserName)?.Value ?? string.Empty;
    public static string GetFirstName(this ClaimsPrincipal principal)
        => principal.FindFirst(AppClaims.FirstName)?.Value ?? string.Empty;
    public static string GetLastName(this ClaimsPrincipal principal)
        => principal.FindFirst(AppClaims.LastName)?.Value ?? string.Empty;

    public static Uri? GetImageUrl(this ClaimsPrincipal principal)
    {
        var imageUrl = principal.FindFirst(AppClaims.ImageUrl)?.Value ?? string.Empty;
        return Uri.TryCreate(imageUrl, UriKind.Absolute, out var uri) ? uri : null;
    }

    
    // public static string? GetUserId(this ClaimsPrincipal principal)
    //     => principal.FindFirstValue(ClaimTypes.NameIdentifier);
    // public static string? GetEmail(this ClaimsPrincipal principal)
    //     => principal.FindFirstValue(ClaimTypes.Email);
    // public static string? GetFirstName(this ClaimsPrincipal principal)
    //     => principal?.FindFirst(ClaimTypes.Name)?.Value;
    // public static string? GetSurname(this ClaimsPrincipal principal)
    //     => principal?.FindFirst(ClaimTypes.Surname)?.Value;
    // public static string? GetPhoneNumber(this ClaimsPrincipal principal)
    //     => principal.FindFirstValue(ClaimTypes.MobilePhone);
    //
    //
    // public static DateTimeOffset GetExpiration(this ClaimsPrincipal principal) =>
    //     DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(
    //         principal.FindFirstValue(AppClaims.Expiration)));
    //
    // private static string? FindFirstValue(this ClaimsPrincipal principal, string claimType) =>
    //     principal is null
    //         ? throw new ArgumentNullException(nameof(principal))
    //         : principal.FindFirst(claimType)?.Value;
}
