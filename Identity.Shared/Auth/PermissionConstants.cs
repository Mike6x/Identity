namespace Identity.Shared.Auth;

/// <summary>
/// Constants for Auth0 authorization permissions.
/// </summary>
public class PermissionConstants
{
    /// <summary>
    /// Permission to read users.
    /// </summary>
    public const string ReadUser = "read:user";

    /// <summary>
    /// Permission to read weather conditions.
    /// </summary>
    public const string ReadWeather = "read:weather";
}