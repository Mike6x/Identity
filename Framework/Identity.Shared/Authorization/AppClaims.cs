namespace Identity.Shared.Authorization;

public static class AppClaims
{
    public const string Tenant = "tenant";
   
    public const string UserId = "sub";
    public const string UserName = "username";
    public const string Email = "email";
    public const string FirstName = "name";
    public const string LastName = "family_name";
    
    public const string Permission = "permission";
    public const string Role = "role";
    public const string ImageUrl = "image_url";
    public const string IpAddress = "ipAddress";
    public const string Expiration = "exp";
}