namespace Identity.Core.Constants;
public static class OpenIddictConstants
{
    public static class Scopes
    {
        public const string Profile = "profile";
        public const string Email = "email";
        public const string Roles = "roles";
        public const string Api = "api";
    }

    public static class ClientTypes
    {
        public const string Web = "web";
        public const string Native = "native";
        public const string Machine = "machine";
    }

    public static class Claims
    {
        public const string Permission = "permission";
        public const string Role = "role";
    }
}
