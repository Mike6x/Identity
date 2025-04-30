// Core/Constants/CacheConstants.cs
namespace Identity.Core.Constants;

public static class CacheConstants
{
    // Prefix
    private const string Prefix = "identity_server";

    // OpenIddict cache key'leri
    public static string OpenIddictConfiguration => $"{Prefix}_openiddict_configuration";
    public static string OpenIddictScope(string scopeName) => $"{Prefix}_openiddict_scope_{scopeName}";
    public static string OpenIddictApplication(string clientId) => $"{Prefix}_openiddict_app_{clientId}";

    // Kullanıcı cache key'leri
    public static string UserClaims(string userId) => $"{Prefix}_user_claims_{userId}";
    public static string UserRoles(string userId) => $"{Prefix}_user_roles_{userId}";
    public static string UserProfile(string userId) => $"{Prefix}_user_profile_{userId}";
    public static string UserPermissions(string userId) => $"{Prefix}_user_permissions_{userId}";

    // Token cache key'leri
    public static string RefreshToken(string token) => $"{Prefix}_refresh_token_{token}";
    public static string AccessToken(string token) => $"{Prefix}_access_token_{token}";

    // Cache süreleri
    public static class ExpirationTimes
    {
        // OpenIddict süreleri
        public static TimeSpan Configuration => TimeSpan.FromHours(24);
        public static TimeSpan Scope => TimeSpan.FromHours(12);
        public static TimeSpan Application => TimeSpan.FromHours(12);

        // Kullanıcı süreleri
        public static TimeSpan UserClaims => TimeSpan.FromMinutes(30);
        public static TimeSpan UserRoles => TimeSpan.FromMinutes(30);
        public static TimeSpan UserProfile => TimeSpan.FromMinutes(15);
        public static TimeSpan UserPermissions => TimeSpan.FromMinutes(30);

        // Token süreleri
        public static TimeSpan RefreshToken => TimeSpan.FromDays(7);
        public static TimeSpan AccessToken => TimeSpan.FromMinutes(30);
    }
}