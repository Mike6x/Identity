namespace Identity.Shared.Authorization;

public static class TenantConstants
{
    public static class Root
    {
        public const string Id = "Superuser";
        public const string Name = "Superuser";
        public const string EmailAddress = "sadmin@root.com";
        public const string DefaultProfilePicture = "assets/defaults/profile-picture.webp";
    }

    public const string DefaultPassword = "123PassW0rd!";

    public const string Identifier = "tenant";
}