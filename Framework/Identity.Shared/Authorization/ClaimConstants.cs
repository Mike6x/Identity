namespace Identity.Shared.Authorization;

/// <summary>
/// Constants for authorization claims.
/// </summary>
public class ClaimConstants
{
    /// <summary>
    /// Permissions from Auth0 as claim.
    /// </summary>
    public const string Permissions = "permissions";
    public const string ReadOnlyClaim = "identity_read_only";
    public const string ReadWriteClaim = "identity_read_write";     
}