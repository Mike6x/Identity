namespace Identity.Shared.Authorization
{
    public static class AppPolicies
    {
        public const string AuthPolicy = nameof(AuthPolicy);
        
        public const string CanManageApplications = nameof(CanManageApplications);
        public const string CanManageScopes = nameof(CanManageScopes);
        public const string CanManageUsers = nameof(CanManageUsers);
        public const string CanManageRoles = nameof(CanManageRoles);       
    }
}
